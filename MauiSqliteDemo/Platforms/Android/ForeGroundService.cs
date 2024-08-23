using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using MauiSqliteDemo;
using Newtonsoft.Json;


[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
public class ForegroundService : Service
{
    private LocalDbService? _localDbService;
    private bool _isRunning;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly HttpClient _httpClient = new HttpClient();


    public override IBinder? OnBind(Intent intent) => null;


    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        //CreateNotificationChannel();

        var notification = new NotificationCompat.Builder(this, "foreground_service_channel")
            .SetContentTitle("Foreground Service")
            .SetContentText("Service is running...")
            //.SetSmallIcon(Resource.Drawable.icon)
            .SetOngoing(true)
            .Build();

        StartForeground(1, notification);

        _localDbService = new LocalDbService();
        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        DoBackgroundWorkAsync(_cancellationTokenSource.Token);

        return StartCommandResult.Sticky;
    }


    async void DoBackgroundWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var comments = await FetchCommentsAsync();
                var commentsToInsert = comments.Take(5).ToList();

                foreach (var comment in commentsToInsert)
                {
                    await _localDbService.Create(comment);
                    Log.Debug("ForegroundService", $"Added comment with ID {comment.Id} to the database.");
                }
            }
            catch (TaskCanceledException)
            {
                Log.Debug("ForegroundService", "Task was canceled.");
            }
            catch (Exception ex)
            {
                Log.Error("ForegroundService", $"Error fetching or saving comments: {ex.Message}");
            }

            try
            {
                await Task.Delay(10000, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Log.Debug("ForegroundService", "Task delay was canceled.");
            }
        }
    }

    async Task<List<Comment>> FetchCommentsAsync()
    {
        var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/comments");
        return JsonConvert.DeserializeObject<List<Comment>>(response);
    }


    public override void OnDestroy()
    {
        _isRunning = false;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _httpClient.Dispose();
        base.OnDestroy();
    }


    //void CreateNotificationChannel()
    //{
    //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
    //    {
    //        var channel = new NotificationChannel(
    //            "foreground_service_channel",
    //            "Foreground Service Channel",
    //            NotificationImportance.Default);

    //        var manager = (NotificationManager)GetSystemService(NotificationService);
    //        manager.CreateNotificationChannel(channel);
    //    }
    //}
}

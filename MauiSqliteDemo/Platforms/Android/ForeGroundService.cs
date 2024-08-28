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
    private HttpClient _httpClient;

    // Called when the service is created
    public override void OnCreate()
    {
        base.OnCreate();
        // Initialize the HTTP client with Sentry error tracking
        var httpHandler = new SentryHttpMessageHandler();
        _httpClient = new HttpClient(httpHandler);
    }

    public override IBinder? OnBind(Intent intent) => null;

    // Called when the service is started
    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        // Create and display a notification for the foreground service
        var notification = new NotificationCompat.Builder(this, "foreground_service_channel")
            .SetContentTitle("Foreground Service")
            .SetContentText("Service is running...")
            .SetOngoing(true) // Prevents the notification from being swiped away
            .Build();

        // Start the service in the foreground with the notification
        StartForeground(1, notification);

        // Initialize the database service and set the running flag
        _localDbService = new LocalDbService();
        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        // Start the background work asynchronously
        DoBackgroundWorkAsync(_cancellationTokenSource.Token);

        // Ensure the service remains running
        return StartCommandResult.Sticky;
    }

    // Background task that repeatedly fetches comments and saves them to the database
    async void DoBackgroundWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Fetch comments from the API
                var comments = await FetchCommentsAsync();
                // Take the first 5 comments for insertion
                var commentsToInsert = comments.Take(5).ToList();

                // Insert each comment into the database
                foreach (var comment in commentsToInsert)
                {
                    await _localDbService.Create(comment);
                    Log.Debug("ForegroundService", $"Added comment with ID {comment.Id} to the database.");
                }
            }
            catch (TaskCanceledException ex)
            {
                // Handle task cancellation
                Log.Debug("ForegroundService", "Task was canceled.");
                SentrySdk.CaptureException(ex);  // Log to Sentry
            }
            catch (Exception ex)
            {
                // Handle other errors
                Log.Error("ForegroundService", $"Error fetching or saving comments: {ex.Message}");
                SentrySdk.CaptureException(ex);  // Log to Sentry
            }

            try
            {
                // Wait for 10 seconds before the next iteration
                await Task.Delay(10000, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                // Handle delay cancellation
                Log.Debug("ForegroundService", "Task delay was canceled.");
                SentrySdk.CaptureException(ex);  // Log to Sentry
            }
        }
    }

    // Fetch comments from the remote API
    private async Task<List<Comment>> FetchCommentsAsync()
    {
        // Send a GET request to the API
        var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/comments");
        // Deserialize the JSON response into a list of Comment objects
        return JsonConvert.DeserializeObject<List<Comment>>(response);
    }

    // Called when the service is destroyed
    public override void OnDestroy()
    {
        // Stop the running task and clean up resources
        _isRunning = false;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _httpClient.Dispose();
        base.OnDestroy();
    }
}

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using MauiSqliteDemo;

[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
public class ForegroundService : Service
{
    private LocalDbService _localDbService;

    public override IBinder OnBind(Intent intent) => null;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();

        var notification = new NotificationCompat.Builder(this, "foreground_service_channel")
            .SetContentTitle("Foreground Service")
            .SetContentText("Service is running...")
            //.SetSmallIcon(Resource.Drawable.icon)
            .SetOngoing(true)
            .Build();

        StartForeground(1, notification);

        // Initialize the database service
        _localDbService = new LocalDbService();

        // Put your background task here
        DoBackgroundWork();

        return StartCommandResult.Sticky;
    }

    void DoBackgroundWork()
    {
        // Example: Add customers in a new thread
        Task.Run(async () =>
        {
            for (int i = 0; i < 1000; i++) // Adjust the number of iterations as needed
            {
                var customer = new Customer
                {
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@example.com",
                    Mobile = $"123456789{i}"
                };

                await _localDbService.Create(customer);

                Log.Debug("ForegroundService", $"Added customer {i} to the database.");
                Thread.Sleep(1000); // Simulate some delay
            }
        });
    }

    void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                "foreground_service_channel",
                "Foreground Service Channel",
                NotificationImportance.Default);

            var manager = (NotificationManager)GetSystemService(NotificationService);
            manager.CreateNotificationChannel(channel);
        }
    }
}

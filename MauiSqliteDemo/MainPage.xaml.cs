using System.Reflection;
#if ANDROID
using Android.App;
using Android.Content;
#endif

namespace MauiSqliteDemo
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;


        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService; // Assign the passed-in database service to a local variable
            // Load the list of comments asynchronously and set them as the data source for the ListView
            Task.Run(async () => listView.ItemsSource = await _dbService.GetComments());


            var assembly = Assembly.GetExecutingAssembly();

            var resourceNames = assembly.GetManifestResourceNames();


        }

        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            listView.ItemsSource = await _dbService.GetComments();
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            await _dbService.ClearAll();
            listView.ItemsSource = await _dbService.GetComments();
        }

        private void OnStartServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StartService(intent);
            SentrySdk.CaptureMessage("Hello Sentry");
#endif
        }

        private async void OnStopServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StopService(intent);
#endif
            listView.ItemsSource = await _dbService.GetComments();
        }

        private void OnCheckServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            bool isRunning = IsServiceRunning(typeof(ForegroundService));
            DisplayAlert("Service Status", isRunning ? "Service is running" : "Service is not running", "OK");
#endif
        }

        private bool IsServiceRunning(System.Type serviceType)
        {
#if ANDROID
            // Get the activity manager to check running services
            ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);
            var runningServices = manager.GetRunningServices(int.MaxValue);

            // Iterate through the running services to see if the specified service is active
            foreach (var service in runningServices)
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(serviceType).CanonicalName))
                {
                    return true;
                }
            }
#endif
            return false;
        }
    }
}

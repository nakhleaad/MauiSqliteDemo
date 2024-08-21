using System.Reflection;
#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace MauiSqliteDemo
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private int _editCustomerId;
        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            Task.Run(async () => listView.ItemsSource = await _dbService.GetCustomers());

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var name in resourceNames)
            {
                Console.WriteLine(name);
            }
        }



        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            if (_editCustomerId == 0)
            {

                await _dbService.Create(new Customer
                {
                    CustomerName = nameEntryField.Text,
                    Email = emailEntryField.Text,
                    Mobile = mobileEntryField.Text
                });

            }
            else
            {
                await _dbService.Update(new Customer
                {
                    Id = _editCustomerId,
                    CustomerName = nameEntryField.Text,
                    Email = emailEntryField.Text,
                    Mobile = mobileEntryField.Text
                });
                _editCustomerId = 0;
            }
            nameEntryField.Text = string.Empty;
            emailEntryField.Text = string.Empty;
            mobileEntryField.Text = string.Empty;

            listView.ItemsSource = await _dbService.GetCustomers();



        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await _dbService.ClearAll();
            listView.ItemsSource = await _dbService.GetCustomers();


        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var customer = (Customer)e.Item;
            var action = await DisplayActionSheet("Actions", "Cancel", null, "Edit", "Delete");
            switch (action)
            {
                case "Edit":
                    _editCustomerId = customer.Id;
                    nameEntryField.Text = customer.CustomerName;
                    emailEntryField.Text = customer.Email;
                    mobileEntryField.Text = customer.Mobile;
                    break;
                case "Delete":
                    await _dbService.Delete(customer);
                    listView.ItemsSource = await _dbService.GetCustomers();
                    break;
            }

        }
        private void OnStartServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
        var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
        Android.App.Application.Context.StartService(intent);
#endif
        }

        private async void OnStopServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
        var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
        Android.App.Application.Context.StopService(intent);

#endif
            listView.ItemsSource = await _dbService.GetCustomers();

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
        ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);
        var runningServices = manager.GetRunningServices(int.MaxValue);

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

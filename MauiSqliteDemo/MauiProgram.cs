using Microsoft.Extensions.Logging;

namespace MauiSqliteDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder
   .UseMauiApp<App>()

   // Add this section anywhere on the builder:
   .UseSentry(options =>
   {
       // The DSN is the only required setting.
       options.Dsn = "https://86368bd0bf4a70790b0a393cbfc8c82c@o4507847940505600.ingest.de.sentry.io/4507847942799440";

       // Use debug mode if you want to see what the SDK is doing.
       // Debug messages are written to stdout with Console.Writeline,
       // and are viewable in your IDE's debug console or with 'adb logcat', etc.
       // This option is not recommended when deploying your application.
       options.Debug = true;

       // Set TracesSampleRate to 1.0 to capture 100% of transactions for tracing.
       // We recommend adjusting this value in production.
       options.TracesSampleRate = 1.0;

       // Other Sentry options can be set here.
   });

            // ... the remainder of your MAUI app setup
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

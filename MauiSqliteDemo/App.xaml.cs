namespace MauiSqliteDemo
{
    public partial class App : Application
    {
        public static LocalDbService DbService { get; private set; }

        public App(MainPage mainPage)
        {
            InitializeComponent();

            MainPage = mainPage;
            DbService = new LocalDbService();

        }
    }
}

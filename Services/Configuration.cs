namespace ChesnokMessengerAPI.Services
{
    public class AppConfiguration
    {
        private static AppConfiguration _instance;

        public IConfigurationRoot Configuration;
        private AppConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile("appsettings.json");
            Configuration = configBuilder.Build();
        }

        public static AppConfiguration GetInstance()
        {
            if (_instance == null)
                _instance = new AppConfiguration();
            return _instance;
        }

    }
}

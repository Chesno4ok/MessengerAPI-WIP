namespace ChesnokMessengerAPI.Services
{
    // TODO: Add a file with a connection string
    static class DatabaseConnectionService
    {
        public static string ConnectionString { 
            get 
            {
                if (System.IO.File.Exists("ConnectionString.txt"))
                {
                    System.IO.File.Create("ConnectionString.txt");
                    throw new FileNotFoundException("Connection String file was not found!");
                }

                return System.IO.File.ReadAllText("ConnectionString.txt");
            } 
        }
    }
}

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChesnokMessengerAPI.WebSockets
{
    public class DbNotification<T>
    {
        public DbNotification()
        {

        }
        public string table { get; set; }
        public string action { get; set; }
        public T data { get; set; }
    }
}

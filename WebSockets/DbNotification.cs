using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChesnokMessengerAPI.WebSockets
{
    public class DbNotification
    {
        public string table { get; set; }
        public string action { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public int Id { get; set; }
        public int User { get; set; }
        public int ChatId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int IsRead { get; set; }
    }
}

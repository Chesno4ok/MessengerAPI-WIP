using System.Reflection.Metadata;
using System.Text;

namespace ChesnokMessengerAPI.Responses
{

    public class MessageResponse
    {
        public MessageResponse(Message msg)
        {
            Id = msg.Id;
            User = msg.User;
            ChatId = msg.ChatId;
            Date = msg.Date;
            Content = msg.Content;
            Type = msg.Type;
            IsRead = msg.IsRead;
        }
        public int Id { get; set; }
        public int User { get; set; }
        public int ChatId { get; set; }
        public DateTime Date { get; set; }
        public byte[]? Content { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
    }
}

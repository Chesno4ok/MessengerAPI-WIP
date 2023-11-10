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
        }
        public int Id { get; set; }

        public int User { get; set; }

        public int ChatId { get; set; }

        public DateTime Date { get; set; }

        public string? Content { get; set; }
    }
}

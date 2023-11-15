namespace ChesnokMessengerAPI.Responses
{
    public class ChatResponse
    {
        public ChatResponse(Chat chat)
        {
            Id = chat.Id;
            ChatId = chat.ChatId;
            User = chat.ChatId;
        }

        public int Id { get; set; }

        public int ChatId { get; set; }

        public int User { get; set; }
    }
}

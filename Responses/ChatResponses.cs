namespace ChesnokMessengerAPI.Responses
{
    public class ChatResponse
    {
        public ChatResponse(Chat chat, int UserId)
        {
            Users = new();
            Messages = new();

            var context = new MessengerApiContext();

            ChatId = chat.Id;
            ChatName = chat.ChatName;

            ChatUser[] users = context.ChatUsers.Where(i => i.Chat == chat).ToArray();

            foreach (ChatUser i in users)
            {
                i.HasUpdates = false;

                Users.Add(new ChatUserResponse(i)); 
            }

            Message[] messages = context.Messages.Where(i => i.ChatId == ChatId).ToArray();

            foreach (var i in messages)
            {
                if (i.User != UserId)
                {
                    i.IsRead = true;
                }

                Messages.Add(new MessageResponse(i));
            }

            context.SaveChanges();
        }

        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public List<ChatUserResponse> Users { get; set; }
        public List<MessageResponse> Messages { get; set; }
    }

    public class ChatUserResponse
    {
        public ChatUserResponse(ChatUser chat)
        {
            Id = chat.Id;
            ChatId = chat.ChatId;
            User = chat.UserId;

            var context = new MessengerApiContext();
            UserName = context.Users.FirstOrDefault(i => i.Id == chat.UserId).Name;
        }

        public int Id { get; set; }
        public int ChatId { get; set; }
        public int User { get; set; }
        public string UserName { get; set; }
        
    }

}

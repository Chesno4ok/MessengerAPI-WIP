using AutoMapper;
using ChesnokMessengerAPI.Services;
using ChesnokMessengerAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ChesnokMessengerAPI.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IMapper _mapper;
        private List<ClientGroup> _clientGroups;
        public ChatHub()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppMappingProfile());
            });

            _mapper = mapperConfig.CreateMapper();
            _clientGroups = new();
            
        }
        [Authorize]
        public async Task SendMessage(MessageTemplate messageTemplate)
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            Clients.Groups(messageTemplate.ChatId.ToString()).SendAsync("ReceiveMessage", messageTemplate);

            dbContext.Messages.Add(_mapper.Map<Message>(messageTemplate));
        }
        [Authorize]
        public async Task SendMessageEdit(EditMessageTemplate messageTemplate)
        {
            using var dbContext = new MessengerContext();

            var editMessage = dbContext.Messages.FirstOrDefault(i => i.Id == messageTemplate.Id)!;
            editMessage.Text = messageTemplate.Text;


            Claim? IdClaim = Context.User!.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            await Clients.Groups(editMessage.ChatId.ToString()).SendAsync("ReceiveMessage", messageTemplate);
            await dbContext.SaveChangesAsync();
        }
        [Authorize]
        public async Task CreateChat(ChatTemplate chatTemplate)
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var newChat = _mapper.Map<Chat>(chatTemplate);
            dbContext.Chats.Add(newChat);
            ChatUser[] chatUsers = _mapper.Map<ChatUser[]>(chatTemplate.ChatUsers);
            dbContext.ChatUsers.AddRange(chatUsers);
            await dbContext.SaveChangesAsync();

            AddUsersToGroups(chatUsers);
        }
        private async Task AddUsersToGroups(IEnumerable<ChatUser> chatUsers)
        {
            using var dbContext = new MessengerContext();
            var chatId = chatUsers.First().ChatId.ToString();

            // Removing clients

            var removedClients = _clientGroups.Where(
                cg => cg.ChatId == chatId && 
                chatUsers.Any(cu => cu.UserId != cg.UserId)
            );

            //var removedClients = _clientGroups.Where(cg => cg.ChatId == chatUsers.First().ChatId.ToString() && chatUsers.Any(cu => cu.UserId != cg.UserId)).ToArray();

            List<Task> taskList = new List<Task>();
            foreach (var client in removedClients)
            {
                var res = Groups.RemoveFromGroupAsync(client.ConnectionId, client.ChatId);
                _clientGroups.Remove(client);
                taskList.Add(res);
            }
            Task.WaitAll(taskList.ToArray());
            taskList.Clear();


            // Adding new clients
            var newChatUsers = chatUsers.Where(cu => _clientGroups.Any(cg => cg.UserId != cu.UserId && cg.ChatId == null));

            foreach (var client in newChatUsers)
            {
                var newClient = _clientGroups.FirstOrDefault(i => i.UserId == client.UserId);

                _clientGroups.Add(new ClientGroup(newClient.ConnectionId, newClient.ChatId, newClient.UserId));
                var res = Groups.AddToGroupAsync(newClient.ConnectionId, newClient.ChatId);
            }
            Task.WaitAll(taskList.ToArray());

            var removedChatUsers = dbContext.ChatUsers.Where(i => i.ChatId == chatUsers.First().ChatId);

            if (removedChatUsers.Count() != 0)
                dbContext.ChatUsers.RemoveRange(removedChatUsers);

            dbContext.ChatUsers.AddRange(chatUsers);
            await dbContext.SaveChangesAsync();
        }

        public override async Task OnConnectedAsync()
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var chatUsers = dbContext.ChatUsers.Where(i => i.UserId == userId);

            List<Task> taskList = new List<Task>();
            foreach(var chat in chatUsers)
            {
                _clientGroups.Add(new ClientGroup(Context.ConnectionId, chat.Id.ToString(), userId));
                _clientGroups.Add(new ClientGroup(Context.ConnectionId, userId));

               var res = Groups.AddToGroupAsync(Context.ConnectionId, chat.ChatId.ToString());
               taskList.Add(res);
            }
            Task.WaitAll(taskList.ToArray());

            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var chatUsers = dbContext.ChatUsers.Where(i => i.UserId == userId);

            List<Task> taskList = new List<Task>();
            _clientGroups.RemoveAll(i => i.ConnectionId == Context.ConnectionId);
            foreach (var chat in chatUsers)
            {
                var res = Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.ChatId.ToString());
                taskList.Add(res);
            }
            Task.WaitAll(taskList.ToArray());

            

            await base.OnDisconnectedAsync(exception);
        }
    }
    class ClientGroup
    {
        public string ConnectionId { get; set; }
        public string? ChatId { get; set; }
        public int UserId { get; set; }

        public ClientGroup(string connectionId, string? chatId, int userId)
        {
            ConnectionId = connectionId;
            ChatId = chatId;
            UserId = userId;
        }

        public ClientGroup(string connectionId, int userId)
        {

        }
    }
}

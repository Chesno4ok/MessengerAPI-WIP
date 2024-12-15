using AutoMapper;
using ChesnokMessengerAPI.Models;
using ChesnokMessengerAPI.Services;
using ChesnokMessengerAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ChesnokMessengerAPI.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IMapper _mapper { get; set; }
        static object locker = new object();
        private static List<ClientGroup> _cg = new();
        private static List<ClientGroup> _clientGroups
        {
            get
            {
                lock (locker)
                {
                    return _cg;
                }
            }
            set
            {
                lock (locker)
                {
                    _cg = value;
                }
            }

        } 
       public ChatHub()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppMappingProfile());
            });

            _mapper = mapperConfig.CreateMapper();
            
        }
        [Authorize]
        public async Task SendMessage(MessageTemplate messageTemplate)
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var newMessage = _mapper.Map<Message>(messageTemplate);
            newMessage.Date = DateTime.UtcNow;

            dbContext.Messages.Add(newMessage);
            await dbContext.SaveChangesAsync();

            newMessage = dbContext.Messages.Include(i => i.User).FirstOrDefault(i => i.Id == newMessage.Id);

            var messageResponse = _mapper.Map<MessageResponse>(newMessage);

            Clients.Groups(messageTemplate.ChatId.ToString()).SendAsync("ReceiveMessage", messageResponse);
        }
        [Authorize]
        public async Task EditMessage(EditMessageTemplate messageTemplate)
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
            await dbContext.SaveChangesAsync();

            ChatUser[] chatUsers = _mapper.Map<ChatUser[]>(chatTemplate.ChatUsers);
            foreach(var chat in chatUsers)
            {
                chat.ChatId = newChat.Id;
            }

            await dbContext.SaveChangesAsync();

            await AddUsersToGroups(chatUsers, newChat.Id.ToString());

            var chatResponse = _mapper.Map<ChatResponse>(newChat);
            Clients.Groups(newChat.Id.ToString()).SendAsync("ReceiveChat", chatResponse);
        }
        [Authorize]
        public async Task EditChat(EditChatTemplate chatTemplate)
        {
            // Editing chat
            using var dbContext = new MessengerContext();

            var chat = dbContext.Chats.FirstOrDefault(i => i.Id == chatTemplate.Id);

            chat = (Chat)_mapper.Map(chatTemplate, chat, typeof(EditChatTemplate), typeof(Chat));

            var deletedChatUsers = dbContext.ChatUsers.Where(i => i.ChatId == chat.Id);

            if (deletedChatUsers.Count() != 0)
            {
                dbContext.ChatUsers.RemoveRange(deletedChatUsers);
            }

            ChatUser[] newChatUsers = _mapper.Map<ChatUser[]>(chatTemplate.ChatUsers);

            await dbContext.SaveChangesAsync();

            // Editing groups

            var newChat = dbContext.Chats.Include(i => i.ChatUsers).FirstOrDefault(i => i.Id == chatTemplate.Id);
            await AddUsersToGroups(newChat.ChatUsers, chat.Id.ToString());


            await Clients.Groups(chatTemplate.Id.ToString()).SendAsync("ReceiveChat", _mapper.Map<ChatResponse>(newChat));

        }
        [Authorize]
        public async Task LeaveChat(int chatId)
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var chatUser = dbContext.ChatUsers.FirstOrDefault(i => i.ChatId == chatId && i.UserId == userId);

            if (chatUser == null)
                return;

            dbContext.ChatUsers.Remove(chatUser);
            await dbContext.SaveChangesAsync();

            await Clients.Group(chatId.ToString()).SendAsync("UserLeft", _mapper.Map<ChatUserResponse>(chatUser));

            var chat = dbContext.Chats.Include(i => i.ChatUsers)
                .FirstOrDefault(i => i.Id == chatId);

            await AddUsersToGroups(chat.ChatUsers, chatId.ToString());
        }
        private async Task AddUsersToGroups(IEnumerable<ChatUser> chatUsers, string chatId)
        {
            // Removing clients

            var removedClients = _clientGroups.Where(
                cg => cg.ChatId == chatId &&
                !chatUsers.Any(cu => cu.UserId != cg.UserId)
            ).ToArray();

            foreach (var client in removedClients)
            {
                await Groups.RemoveFromGroupAsync(client.ConnectionId, client.ChatId);
                _clientGroups.Remove(client);
            }

            // Adding new clients
            var newChatUsers = chatUsers.Where(cu => _clientGroups.Any(cg => cg.UserId == cu.UserId)).DistinctBy(i => i.UserId);

            foreach (var client in newChatUsers)
            {
                var newClient = _clientGroups.FirstOrDefault(i => i.UserId == client.UserId);

                _clientGroups.Add(new ClientGroup(newClient.ConnectionId, chatId, newClient.UserId));
                await Groups.AddToGroupAsync(newClient.ConnectionId, chatId);
            }
        }
        public override async Task OnConnectedAsync()
        {
            using var dbContext = new MessengerContext();

            Claim? IdClaim = Context.User.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (IdClaim == null)
                return;

            int userId = Int32.Parse(IdClaim.Value);

            var chatUsers = dbContext.ChatUsers.Where(i => i.UserId == userId);

            foreach(var chat in chatUsers)
            {
               _clientGroups.Add(new ClientGroup(Context.ConnectionId, chat.Id.ToString(), userId));
                
               Groups.AddToGroupAsync(Context.ConnectionId, chat.ChatId.ToString());
               
            }

            _clientGroups.Add(new ClientGroup(Context.ConnectionId, userId));

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
            ConnectionId = connectionId;
            ChatId = "";
            UserId = userId;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ChesnokMessengerAPI.Responses;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using AutoMapper;
using ChesnokMessengerAPI.Templates;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChesnokMessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MessengerContext _context;
        private IMapper _mapper;

        public ChatController(IMapper mapper)
        {
            _mapper = mapper;
            _context = new MessengerContext();
        }
        // Create a new chat with users
        [HttpPost("create_chat")]
        public IActionResult CreateChat(ChatTemplate chatTemplate)
        {
            // Creating new chat
            var context = new MessengerContext();

            if (chatTemplate.Id != null)
                return BadRequest(new InvalidParametersResponse("Error", "Id must be null", new string[] { "Id" }));

            var newChat = _mapper.Map<Chat>(chatTemplate);

            context.Add(newChat);

            context.SaveChanges();

            // Adding chatUsers to chat
            // TODO: Проверить, валидируються ли так указанные пользователи
            ChatUser[] chatUsers = _mapper.Map<ChatUser[]>(chatTemplate.ChatUsers);

            context.ChatUsers.AddRange(chatUsers);
            context.SaveChanges();

            return Ok();
        }
        // Add users to the chat
#warning
        // TODO: Проверить, является ли добавляющий пользователя участник, участником чата
        [HttpPost("add_user")]
        public IActionResult AddUser(ChatUserTemplate chatUserTemplate)
        {
            var context = new MessengerContext();

            if (chatUserTemplate.Id != null)
                return BadRequest(new InvalidParametersResponse("Error", "Id must be null", new string[] { "Id" }));

            var chatUser = _mapper.Map<ChatUser>(chatUserTemplate);
            context.ChatUsers.Add(chatUser);

            context.SaveChanges();
            return Ok();
        }
        // Get all chats that user is part of
        [HttpGet("get_chats")]
        public IActionResult GetChats(int userId)
        {
            var context = new MessengerContext();
            Chat[] chats = context.ChatUsers.Where(i => i.UserId == userId).Include(i => i.Chat).Select(i => i.Chat).ToArray();

            return Ok(chats.ToJson());
        }
        // Get certain chat
        [HttpGet("get_chat")]
        public  IActionResult GetChat(int chatId)
        {
            var context = new MessengerContext();
            var chat = context.Chats.FirstOrDefault(i => i.Id == chatId);

            return Ok(chat.ToJson());
        }

        [HttpGet("get_users")]
        public IActionResult GetUsers(int chatId)
        {
            var context = new MessengerContext();

            var users = context.ChatUsers.Where(i => i.ChatId == chatId).Include(i => i.User).Select(i => i.User);

            return Ok(users.ToJson());
        }
        [HttpPost("update_chat")]
        public IActionResult UpdateUsers(ChatTemplate chatTemplate)
        {
            using var context = new MessengerContext();

            if (chatTemplate.Id == null)
                return BadRequest(new InvalidParametersResponse("Error", "Id cannot be null", new string[] { "Id" }));

            var chat = context.Chats.FirstOrDefault(i => i.Id == chatTemplate.Id);

            chat = (Chat)_mapper.Map(chatTemplate, chat, typeof(ChatTemplate), typeof(Chat));

            var deletedChatUsers = context.ChatUsers.Where(i => i.ChatId == chat.Id);

            if(deletedChatUsers.Count() != 0)
            {
                context.ChatUsers.RemoveRange(deletedChatUsers);
            }

            ChatUser[] newChatUsers = _mapper.Map<ChatUser[]>(chatTemplate.ChatUsers);

            return Ok();
        }
        [HttpPost("leave_chat")]
        public IActionResult LeaveGroup(int userId, int chatId)
        {
            using var context = new MessengerContext();

            var removedChatUser = context.ChatUsers.FirstOrDefault(i => i.ChatId == chatId && i.UserId == userId);
            context.ChatUsers.Remove(removedChatUser);

            context.SaveChanges();


            return Ok();
        }
    }
}

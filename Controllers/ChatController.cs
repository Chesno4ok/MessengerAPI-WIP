using Microsoft.AspNetCore.Mvc;
using ChesnokMessengerAPI.Responses;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ChesnokMessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MessengerContext _context;

        public ChatController()
        {
            _context = new MessengerContext();
        }
        // Create a new chat with users
        [HttpPost("create_chat")]
        public IActionResult CreateChat(int userId, string token, string chatName, int[] users)
        {
            var _context = new MessengerContext();
            foreach(int i in users) // Checking incoming users
            {
                var user = _context.Users.FirstOrDefault(i => i.Id == userId);

                if(user == null)
                {
                    return BadRequest(new Response("Error", "Invalid users"));
                }
            }

            var chat = new Chat() { ChatName = chatName };
            _context.Chats.Add(chat);
            _context.SaveChanges();

            _context.ChatUsers.Add(new ChatUser() { ChatId = chat.Id, UserId = userId });
            foreach (int i in users)
            {
                _context.ChatUsers.Add(new ChatUser() { ChatId = chat.Id, UserId = i });
            }
            
            _context.SaveChanges();
            return Ok();
        }
        // Add users to the chat
        [HttpPost("add_user")]
        public IActionResult AddUser(int userId, int chatId, string token, int[] users)
        {
            var _context = new MessengerContext();
            if (users.Any(i => _context.Users.FirstOrDefault(x => x.Id == i) == null))
            {
                return BadRequest(new Response("Error", "One of the users is incorrect"));
            }

            foreach (var i in users)
            {
                _context.ChatUsers.Add(new ChatUser() { ChatId = chatId, UserId = i });
            }

            _context.SaveChanges();
            return Ok();
        }
        // Get all chats that user is part of
        [HttpGet("get_chats")]
        public IActionResult GetChats(int userId, string token)
        {
            var _context = new MessengerContext();
            Chat[] chats = _context.Chats.Where(x => _context.ChatUsers.Where(i => i.UserId == userId).Any(y => y.ChatId == x.Id)).ToArray();

            return Ok(chats.ToJson());
        }
        // Get certain chat
        [HttpGet("get_chat")]
        public  IActionResult GetChat(int userId, string token, int chatId)
        {
            var _context = new MessengerContext();
            var chat = _context.Chats.FirstOrDefault(i => i.Id == chatId);

            return Ok(chat.ToJson());
        }
        // Get certain chat
        [HttpGet("get_chat_amount")]
        public IActionResult GetChatAmount(int userId, string token, int chatId)
        {
            var _context = new MessengerContext();
            var chat = _context.Chats.FirstOrDefault(i => i.Id == chatId);

            return Ok(chat.ToJson());
        }
        
    }
}

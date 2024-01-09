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
        private readonly MessengerApiContext _context;

        public ChatController()
        {
            _context = new MessengerApiContext();
        }
        // Create a new chat with users
        [HttpPost("create_chat")]
        public IActionResult CreateChat(int userId, string token, string chatName, int[] users)
        {
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
            if(users.Any(i => _context.Users.FirstOrDefault(x => x.Id == i) == null))
            {
                return BadRequest(new Response("Error", "One of the users is incorrect"));
            }

            foreach (var i in users)
            {
                _context.ChatUsers.Add(new ChatUser() { ChatId = chatId, UserId = userId });
            }

            _context.SaveChanges();
            return Ok();
        }
        // Get all chats that user is part of
        [HttpGet("get_chats")]
        public IActionResult GetChats(int userId, string token)
        {
            Chat[] chats = _context.Chats.Where(x => _context.ChatUsers.Where(i => i.UserId == userId).Any(y => y.ChatId == x.Id)).ToArray();

            List<ChatResponse> messages = new();

            foreach (var i in chats)
            {
                messages.Add(new ChatResponse(i, userId));
            }

            return Ok(messages);
        }
        [HttpGet("get_chat")]
        public IActionResult GetChat(int userId, string token, int chatId)
        {
            var chat = _context.Chats.FirstOrDefault(i => i.Id == chatId);
            ChatResponse chatResponse = new ChatResponse(chat, userId);

            return Ok(chatResponse);
        }
    }
}

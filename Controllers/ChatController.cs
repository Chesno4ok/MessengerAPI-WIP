using Microsoft.AspNetCore.Mvc;
using ChesnokMessengerAPI.Responses;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("create_chat")]
        public IActionResult CreateChat(int userId, int token, string chatName, int[] users)
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

            foreach(int i in users)
            {
                _context.ChatUsers.Add(new ChatUser() { ChatId = chat.Id, UserId = i });
            }


            return Ok();
        }


        [HttpGet("get_chats")]
        public IActionResult GetChats(int userId, string token)
        {
            List<ChatUser> chats = _context.ChatUsers.Where(i => i.UserId == userId).ToList();

            List<ChatUserResponse> chatResponses = new();
            
            foreach(ChatUser i in chats)
            {
                chatResponses.AddRange(_context.ChatUsers.Where(x => x.ChatId == i.ChatId).ToList().ConvertAll(x => new ChatUserResponse(x)));
            }

            chatResponses.OrderBy(i => i.ChatId);
            return Ok(chatResponses);
        }
    }
}

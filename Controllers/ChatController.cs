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
        public IActionResult CreateChat(int userId, int token, int[] users)
        {
            foreach(int i in users) // Checking incoming users
            {
                var user = _context.Users.FirstOrDefault(i => i.Id == userId);

                if(user == null)
                {
                    return BadRequest(new Response("Error", "Invalid users"));
                }
            }

            foreach(int i in users)
            {
                _context.Chats.Add(new Chat() { ChatId = System.Guid.NewGuid(). })
            }


            return Ok();
        }


        [HttpGet("get_chats")]
        public IActionResult GetChats(int userId, string token)
        {
            List<Chat> chats = _context.Chats.Where(i => i.User == userId).ToList();

            List<ChatResponse> chatResponses = new List<ChatResponse>();
            foreach(Chat i in chats)
            {
                chatResponses.AddRange(_context.Chats.Where(x => x.ChatId == i.ChatId).ToList().ConvertAll(x => new ChatResponse(x)));
            }

            chatResponses.OrderBy(i => i.ChatId);
            return Ok(chatResponses);
        }
    }
}

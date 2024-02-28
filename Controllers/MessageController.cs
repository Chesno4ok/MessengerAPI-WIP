using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ChesnokMessengerAPI.Responses;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;

namespace ChesnokMessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly MessengerApiContext _context;

        public MessageController()
        {
            _context = new MessengerApiContext();
        }
        // Sene a message to the chat
        [HttpPost("send_message")]
        public IActionResult SendTextMessage(int userId, string token, int chatId,  string content, string type)
        {
            var _context = new MessengerApiContext();

            _context.Messages.AddAsync(new Message
            {
                ChatId = chatId,
                User = userId,
                Content = Encoding.Unicode.GetBytes(content),
                Date = DateTime.Now,
                Type = type
            });


            var users = _context.ChatUsers.Where(i => i.ChatId == chatId && i.UserId != userId);

            foreach(var i in users)
            {
                i.HasUpdates = true;
            }


            _context.SaveChanges();
            return Ok();
            
        }
    }
}

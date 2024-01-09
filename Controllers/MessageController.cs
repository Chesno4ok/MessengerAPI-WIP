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
            
            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                User = userId,
                Content = Encoding.Unicode.GetBytes(content),
                Date = DateTime.Now,
                Type = type
            });

            
            _context.SaveChanges();
            return Ok();
            
        }
    }
}

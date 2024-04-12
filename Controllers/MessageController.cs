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

            var message = new Message
            {
                ChatId = chatId,
                User = userId,
                Content = Encoding.Unicode.GetBytes(content),
                Date = DateTime.Now,
                Type = type
            };

            _context.Messages.AddAsync(message);

            var chat = _context.Chats.First(i => i.Id == chatId);
            chat.LastMessageId = _context.Messages.OrderBy(i => i.Date).Last().Id;

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

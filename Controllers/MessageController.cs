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

            var users = _context.ChatUsers.Where(i => i.ChatId == chatId && i.UserId != userId);

            foreach(var i in users)
            {
                i.HasUpdates = true;
            }


            _context.SaveChanges();
            return Ok();
            
        }

        [HttpGet("get_messages")]
        public IActionResult GetMessages(int userId, string token, int chatId, int amount)
        {
            var context = new MessengerApiContext();
            var messages = context.Messages.Where(i => i.ChatId == chatId).OrderBy(i => i.Id).Reverse().ToArray();

            return Ok(messages.ToJson());
        }
        [HttpGet("get_new_messages")]
        public IActionResult GetNewMessages(int userId, string token, int chatId, int lastId, int amount)
        {
            var context = new MessengerApiContext();
            var messages = context.Messages.Where(i => i.ChatId == chatId && i.Id > lastId).Take(amount).ToArray();

            return Ok(messages.ToJson());
        }
        [HttpGet("get_previous_messages")]
        public IActionResult GetPrevMessages(int userId, string token, int chatId, int firstId, int amount)
        {
            var context = new MessengerApiContext();
            var messages = context.Messages.Where(i => i.ChatId == chatId && i.Id < firstId).Take(amount).ToArray();

            return Ok(messages.ToJson());
        }

    }
}

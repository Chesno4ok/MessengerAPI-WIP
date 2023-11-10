using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ChesnokMessengerAPI.Responses;
using System.Text.Json;
using Newtonsoft.Json;

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

        [HttpGet("get_messages")]
        public IActionResult GetMessages(int userId, string token)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId && i.Token == token);
            if (user == null)
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Invalid token"
                });

            List<Chat> chats = _context.Chats.Where(i => i.User == userId).ToList();
            List<Message> msgs = new List<Message>();
            foreach(Chat c in chats)
            {
                msgs.AddRange(_context.Messages.Where(m => m.ChatId == c.ChatId));
            }
            msgs.OrderBy(i => i.Date);
            msgs.ConvertAll(i => i as MessageData);

            return Ok(JsonConvert.SerializeObject(msgs));
        }
        [HttpPost("send_message")]
        public IActionResult SendMessage(int fromId, int chatId, string token, string content)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == fromId && i.Token == token);
            if (user == null)
                return BadRequest(new Response() { 
                    status = "Error", 
                    message = "Invalid token" });


            var chat = _context.Chats.FirstOrDefault(i => i.Id == chatId && i.User == fromId);
            if (chat == null)
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Invalid chatId"
                });



            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                User = fromId,
                Content = content,
                Date = new DateTime()
            });

            return Ok();
        }


        
    }
}

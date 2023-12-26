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

        // TODO: Clean Up this shit
        [HttpGet("get_messages")]
        public IActionResult GetMessages(int userId, string token)
        {
            List<ChatUser> chats = _context.ChatUsers.Where(i => i.UserId == userId).ToList();

            List<Message> msgs = new List<Message>();
            
            foreach(ChatUser c in chats)
            {
                msgs.AddRange(_context.Messages.Where(m => m.ChatId == c.ChatId));
            }
            msgs.OrderBy(i => i.Date);

            List<MessageResponse> messageResponses = msgs.ConvertAll(i => new MessageResponse(i));

            return Ok(messageResponses.ToJson());
        }
        [HttpPost("send_message")]
        public IActionResult SendMessage(int userId, int chatId, string token, string content)
        {
            
            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                User = userId,
                Content = content,
                Date = DateTime.Now
            });

            
            _context.SaveChanges();
            return Ok();
            
        }
    }
}

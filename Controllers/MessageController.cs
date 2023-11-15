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
            List<Chat> chats = _context.Chats.Where(i => i.User == userId).ToList();

            List<Message> msgs = new List<Message>();
            foreach(Chat c in chats)
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
            var chat = _context.Chats.FirstOrDefault(i => i.Id == chatId && i.User == userId);
            if (chat == null)
                return BadRequest(new Response("Error", "Invalid Chat Id").ToJson());



            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                User = userId,
                Content = content,
                Date = DateTime.Now
            });

            try
            {
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(new Response("Error", exc.Message).ToJson());
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ChesnokMessengerAPI.Responses;

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
            var user = _context.Users.FirstOrDefault(i => i.Id == userId && i.UserToken == token);
            if (user == null)
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Invalid token"
                });

            List<Chat> chats = _context.Chats.Where(i => i.User == userId).ToList();

            List<Message> msgs = _context.Messages.Where(i => chats.Any(x => x.ChatId == i.ChatId)).ToList();

            return Ok(msgs);
        }

        [HttpPost("send_message")]
        public IActionResult SendMessage(int fromId, string chatId, string token, string content)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == fromId && i.UserToken == token);
            if (user == null)
                return BadRequest(new Response() { 
                    status = "Error", 
                    message = "Invalid token" });


            var chat = _context.Chats.FirstOrDefault(i => i.ChatId == chatId && i.User == fromId);
            if (chat == null)
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Invalid chatId"
                });



            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                FromUser = fromId,
                Content = content,
                Date = new DateTime()
            });

            return Ok();
        }


        
    }
}

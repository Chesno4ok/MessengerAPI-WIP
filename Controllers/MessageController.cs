using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

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

        [HttpGet("get_updates")]
        public IActionResult GetUpdates(int id, int token)
        {
            List<Message> messages = _context.Messages.Where(i => i.ToUser == id).ToList();

            return Ok(messages);
        }

        [HttpGet("get_messages")]
        public IActionResult GetAllMessages(int id, int token)
        {
            List<Message> messages = _context.Messages.Where(i => i.ToUser == id || i.FromUser == id).OrderBy(i => i.Date).ToList();

            return Ok(messages);
        }


        [HttpPost("send_message")]
        public IActionResult SendMessage(int fromId, int toId, string token, string content)
        {
            var fromUser = _context.Users.FirstOrDefault(i => i.Id == fromId);
            var toUser = _context.Users.FirstOrDefault(i => i.Id == toId);

            if(fromUser == null || toUser == null)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Invalid IDs"
                }.ToJson());
            }

            toUser.HasUpdates = 1;

            _context.Messages.Add(new Message()
            {
                ToUser = toId,
                FromUser = fromId,
                Content = content,
                Date = new DateTime()
            });

            try
            {
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = exc.Message
                }.ToJson());
            }


        }
    }
}

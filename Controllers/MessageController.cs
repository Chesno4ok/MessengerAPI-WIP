using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ChesnokMessengerAPI.Responses;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.WebSockets;
using ChesnokMessengerAPI.WebSockets;
using ChesnokMessengerAPI.Services;
using static System.Net.Mime.MediaTypeNames;

namespace ChesnokMessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly MessengerContext _context;

        public MessageController()
        {
            _context = new MessengerContext();
        }
        // Sene a message to the chat
        [HttpPost("send_text_message")]
        public IActionResult SendTextMessage(int userId, string token, int chatId, string text)
        {
            using var _context = new MessengerContext();

            var message = new Message
            {
                ChatId = chatId,
                User = userId,
                Text = text,
                Date = DateTimeOffset.UtcNow
            };

            _context.Messages.AddAsync(message);

            _context.SaveChanges();
            return Ok();
            
        }
        // Subscribe to new messages with WebSocket
        [Route("ws_exchange_messages")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task ExchangeMessages(int userId, string token)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            User user = new User();
            using(var db = new MessengerContext())
            {
                user = db.Users.FirstOrDefault(i => i.Id == userId);
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var webSocketSession = new WebSocketMessageExchange(webSocket, user);

            while(webSocket.State == WebSocketState.Open) { }
        }
        // Get latest messages
        [HttpGet("get_last_messages")]
        public IActionResult GetLastMessages(int userId, string token, int chatId, int amount)
        {
            var context = new MessengerContext();
            var messages = context.Messages.Where(i => i.ChatId == chatId)
                .ToArray()
                .Reverse()
                .Take(amount);


            return Ok(messages.ToJson());
        }
        [HttpGet("get_previous_messages")]
        public IActionResult GetPreviousMessages(int userId, string token, int chatId, int messageId, int amount)
        {
            var context = new MessengerContext();
            var messages = context.Messages.Where(i => i.Id < messageId && i.ChatId == chatId)
                .ToArray()
                .Reverse()
                .Take(amount);
                


            return Ok(messages.ToJson());
        }
        [HttpPatch("edit_message")]
        public IActionResult EditMessage(int userId, string token, int messageId, string text)
        {
            using(var context = new MessengerContext())
            {
                var message = context.Messages.FirstOrDefault(i => i.Id == messageId);

                message.Text = text;

                context.SaveChanges();
            }

            return Ok();
        }
        [HttpDelete("delete_message")]
        public IActionResult DeleteMessage(int userId, string token, int messageId)
        {
            using (var context = new MessengerContext())
            {
                var message = context.Messages.FirstOrDefault(i => i.Id == messageId);

                context.Remove(message);

                context.SaveChanges();
            }

            return Ok();
        }
    }
}

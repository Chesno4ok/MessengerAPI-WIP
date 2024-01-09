using ChesnokMessengerAPI.Responses;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChesnokMessengerAPI.Middleware
{
    public class ParamCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private MessengerApiContext _dbContext;
        public ParamCheckMiddleware(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerApiContext();
        }

        // Check all for parameters correction in http requests
        public Task InvokeAsync(HttpContext context)
        {
            Dictionary<string, string> query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());

            if(query.ContainsKey("userId"))
            {
                int userId = Convert.ToInt32(query["userId"]);
                var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId);

                if (user == null)
                {
                    SendBadRequst(context, "Invalid userId");
                    return Task.CompletedTask;
                }

            }
            if (query.ContainsKey("userId") && query.ContainsKey("token"))
            {
                int userId = Convert.ToInt32(query["userId"]);
                string token = query["token"];

                var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId && i.Token == token);

                if (user == null)
                {
                    SendBadRequst(context, "Invalid token");
                    return Task.CompletedTask;
                }
            }
            if (query.ContainsKey("userId") && query.ContainsKey("chatId"))
            {
                int userId = Convert.ToInt32(query["userId"]);
                int chatId = Convert.ToInt32(query["chatId"]);

                var chatUser = _dbContext.ChatUsers.FirstOrDefault(i => i.ChatId == chatId && i.UserId == userId);

                if (chatUser == null)
                {
                    SendBadRequst(context, "Invalid chatId");
                    return Task.CompletedTask;
                }
            }
            if (query.ContainsKey("chatId"))
            {
                int chatId = Convert.ToInt32(query["chatId"]);
                Chat? chat = _dbContext.Chats.FirstOrDefault(i => i.Id == chatId);

                if (chat == null)
                {
                    SendBadRequst(context, "Invalid chatId");
                    return Task.CompletedTask;
                }
            }
            if(query.ContainsKey("type"))
            {
                var rx = new Regex(@"^\..*\z");
                string[] types = new string[] { "text", "audio", "photo", "video" };

                if (!rx.IsMatch(query["type"]) && !types.Any(i => i == query["type"]))
                {
                    SendBadRequst(context, "Invalid type");
                    return Task.CompletedTask;
                }
            }
            if(query.ContainsKey("content"))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(query["content"]);

                if(bytes.Length > 2048)
                {
                    SendBadRequst(context, "Content size is too high");
                    return Task.CompletedTask;
                }
            }
            

            _next.Invoke(context);
            return Task.CompletedTask;
        }
        private void SendBadRequst(HttpContext context, string reason)
        {
            context.Response.StatusCode = 400;
            context.Response.WriteAsync(new Response("Error", reason).ToJson());
        }
    }
}

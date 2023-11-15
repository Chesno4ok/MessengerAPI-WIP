using ChesnokMessengerAPI.Responses;
using NuGet.Protocol;
using System.Text.Json;

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
        public Task InvokeAsync(HttpContext context)
        {
            Dictionary<string, string> query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());

            if(query.ContainsKey("userId"))
            {
                int userId = Convert.ToInt32(query["userId"]);
                var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId);

                if (user == null)
                {
                    context.Response.StatusCode = 400;
                    context.Response.WriteAsJsonAsync(new Response("Error", "Invalid userId").ToJson());
                    return Task.CompletedTask;
                }

            }
            if(query.ContainsKey("chatId"))
            {
                int chatId = Convert.ToInt32(query["chatId"]);
                var chat = _dbContext.Chats.FirstOrDefault(i => i.Id == chatId);

                if (chat == null)
                {
                    context.Response.StatusCode = 400;
                    context.Response.WriteAsJsonAsync(new Response("Error", "Invalid chatId").ToJson());
                    return Task.CompletedTask;
                }
            }

            _next.Invoke(context);
            return Task.CompletedTask;
        }
    }
}

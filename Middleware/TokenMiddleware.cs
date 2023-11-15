using ChesnokMessengerAPI.Responses;
using NuGet.Protocol;

namespace ChesnokMessengerAPI.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private MessengerApiContext _dbContext;
        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerApiContext();
        }

        public Task InvokeAsync(HttpContext context)
        {
            Dictionary<string, string> query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());

            if (!(query.ContainsKey("token") && query.ContainsKey("userId")))
            {
                _next.Invoke(context);
                return Task.CompletedTask;
            }

            string tokenValue = query["token"];
            int userId = Convert.ToInt32(query["userId"]);

            var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId && i.Token == tokenValue);
            if(user == null)
            {
                context.Response.StatusCode = 401;
                context.Response.WriteAsJsonAsync(new Response("Error", "Invalid token").ToJson());
                
            }
            else
            {
                _next.Invoke(context);
            }

            return Task.CompletedTask;
        }
    }
}

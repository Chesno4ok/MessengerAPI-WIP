using ChesnokMessengerAPI.Responses;
using NuGet.Common;
using NuGet.Protocol;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChesnokMessengerAPI.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private MessengerContext _dbContext;
        private HttpContext context;
        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerContext();
        }

        public Task InvokeAsync(HttpContext context)
        {
            Dictionary<string, string> query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());
            this.context = context;

            if(!CheckParamChat(query) || 
                !CheckParamUser(query) || 
                !CheckParamUserId(query))
            {
                return Task.CompletedTask;
            }

            _next.Invoke(context);
            return Task.CompletedTask;
        }

        private void SendBadRequest(string expl)
        {
            context.Response.StatusCode = 401;
            context.Response.WriteAsJsonAsync(new Response("Error", expl).ToJson());
        }

        private bool CheckParamUserId(Dictionary<string, string> query)
        {
            string param = "UserId";
            if (query.ContainsKey(param))
            {
                return true;
            }

            var obj = _dbContext.Users.FirstOrDefault(i => i.Id == Convert.ToInt32(query[param]));

            if (obj == null)
            {
                SendBadRequest($"Invalid {param}");
                return false;
            }
            return true;
        }

        private bool CheckParamChat(Dictionary<string, string> query)
        {
            string param = "ChatId";
            if (query.ContainsKey(param))
            {
                return true;
            }

            var obj = _dbContext.Chats.FirstOrDefault(i => i.Id == Convert.ToInt32(query[param]));

            if(obj == null)
            {
                SendBadRequest($"Invalid {param}");
                return false;
            }
            return true;
        }

        private bool CheckParamUser(Dictionary<string, string> query)
        {
            if ((query.ContainsKey("token") && query.ContainsKey("UserId")))
            {
                return true;
            }

            var obj = _dbContext.Users.FirstOrDefault(i => i.Id == Convert.ToInt32(query["UserId"]) && i.Token == query["token"]);

            if (obj == null)
            {
                SendBadRequest("Invalid token");
                return false;
            }
            return true;
        }

        
    }
}

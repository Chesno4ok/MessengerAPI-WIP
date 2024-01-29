using ChesnokMessengerAPI.Responses;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Protocol;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChesnokMessengerAPI.Middleware
{
    public class ParamCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private MessengerApiContext _dbContext;
        private Dictionary<string, string> _query;
        public ParamCheckMiddleware(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerApiContext();
        }

        // Check all for parameters correction in http requests
        public Task InvokeAsync(HttpContext context)
        {
            _query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());

            string[] keys = _query.Keys.ToArray();
            MethodInfo[] methods = GetType().GetMethods();

            bool result = true;
            List<string> invalidParameters = new();

            foreach(var m in methods)
            {
                object[] attributes = m.GetCustomAttributes(typeof(ParameterValidation), false).ToArray();

                foreach(ParameterValidation a in attributes)
                {
                    if (a.parameters.Any(i => keys.Contains(i)))
                    {
                        result = (bool)m.Invoke(this, null);

                        if(!result)
                        {
                            invalidParameters.AddRange(a.parameters);
                        }
                    }
                }

            }

            if (result)
            {
                _next.Invoke(context);
            }
            else
            {
                SendBadRequst(context, "Invalid parameters", invalidParameters.ToArray());
            }
            return Task.CompletedTask;
        }
        private void SendBadRequst(HttpContext context, string reason, string[] parameters)
        {
            context.Response.StatusCode = 400;
            context.Response.WriteAsync(new InvalidParametersResponse("Error", reason, parameters).ToJson());
        }

        [ParameterValidation("userId")]
        public bool Validate_UserId()
        {
            int userId = Convert.ToInt32(_query["userId"]);
            var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId);

            if (user == null)
                return false;
            return true;
        }
        

        [ParameterValidation("userId","chatId")]
        public bool Validate_UserId_ChatId()
        {
            int userId = Convert.ToInt32(_query["userId"]);
            int chatId = Convert.ToInt32(_query["chatId"]);

            var chatUser = _dbContext.ChatUsers.FirstOrDefault(i => i.ChatId == chatId && i.UserId == userId);

            if (chatUser == null)
                return false;
            return true;
        }
        

        [ParameterValidation( "chatId")]
        public bool Validate_ChatId()
        {
            int chatId = Convert.ToInt32(_query["chatId"]);
            Chat? chat = _dbContext.Chats.FirstOrDefault(i => i.Id == chatId);

            if (chat == null)
                return false;
            return true;
        }

        [ParameterValidation("userId","token")]
        public bool Validate_UserId_Token()
        {
            int userId = Convert.ToInt32(_query["userId"]);
            string token = _query["token"];

            var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId && i.Token == token);

            if (user == null)
                return false;
            return true;
        }

        [ParameterValidation("type")]
        public bool Validate_Type()
        {
            var rx = new Regex(@"^\..*\z");
            string[] types = new string[] { "text", "audio", "photo", "video" };

            if (!rx.IsMatch(_query["type"]) && !types.Any(i => i == _query["type"]))
                return false;
            return true;
        }

        [ParameterValidation("content")]
        public bool Validate_Content()
        {
            byte[] bytes = Encoding.Unicode.GetBytes(_query["content"]);

            if (bytes.Length > 2048)
                return false;
            return true;
        }

        [ParameterValidation("login","password")]
        public bool Validate_Login_Password()
        {
            var user = _dbContext.Users.FirstOrDefault(i => i.Login == _query["login"] && i.Password == _query["password"]);

            if (user == null)
                return false;
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ParameterValidation : Attribute
    {
        public string[] parameters;
        public ParameterValidation()
        {

        }
        public ParameterValidation(params string[] parameters) 
        {
            this.parameters = parameters;
        }
    }

}

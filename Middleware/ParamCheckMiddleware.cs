using Azure.Core;
using ChesnokMessengerAPI.Models;
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
{//Login231!
    public class ParamCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private HttpContext _httpContext;
        private MessengerContext _dbContext;
        private Dictionary<string, string> _query;
        public ParamCheckMiddleware(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerContext();
        }

        // Check all for parameters correction in http requests
        public async Task InvokeAsync(HttpContext context)
        {
            
            _query = context.Request.Query.ToDictionary(i => i.Key, i => i.Value.ToString());
            _httpContext = context;


            //MethodInfo[] methods = GetType().GetMethods();

            bool result = true;
            List<string> invalidParameters = new();

            var methods = GetType().GetMethods()
                 .Where(i => i.GetCustomAttribute<ParameterValidation>() != null)
                 .Where(i => i.GetCustomAttribute<ParameterValidation>().parameters.All(i => _query.ContainsKey(i)));
                    

            foreach(var m in methods)
            {
                result = (bool)m.Invoke(this, new object[1] { _query });
                if (!result)
                    invalidParameters.AddRange(m.GetCustomAttribute<ParameterValidation>().parameters);

                

            }

            if (invalidParameters.Count == 0)
                await _next.Invoke(context);
            else
                SendBadRequst(context, "Invalid parameters", invalidParameters.ToArray());

        }

        private void SendBadRequst(HttpContext context, string reason, string[] parameters)
        {
            context.Response.StatusCode = 400;
            context.Response.WriteAsync(new InvalidParametersResponse("Error", reason, parameters).ToJson());
        }

        [ParameterValidation("userId")]
        public bool Validate_UserId(Dictionary<string,string> _query)
        {
            var _dbContext = new MessengerContext();

            int UserId = Convert.ToInt32(_query["userId"]);

            var user = _dbContext.Users.FirstOrDefault(i => i.Id == UserId);

            return user != null;
        }

        [ParameterValidation("userId","chatId")]
        public  bool Validate_UserId_ChatId(Dictionary<string, string> _query)
        {
            var _dbContext = new MessengerContext();

            int UserId = Convert.ToInt32(_query["userId"]);
            int ChatId = Convert.ToInt32(_query["chatId"]);

            var ChatUser = _dbContext.ChatUsers.FirstOrDefault(i => i.ChatId == ChatId && i.UserId == UserId);

            return ChatUser != null;
        }

        [ParameterValidation("chatId")]
        public bool Validate_ChatId(Dictionary<string, string> _query)
        {
            var _dbContext = new MessengerContext();

            int ChatId = Convert.ToInt32(_query["chatId"]);
            Chat? chat =  _dbContext.Chats.FirstOrDefault(i => i.Id == ChatId);

            return chat != null;
        }

        [ParameterValidation("type")]
        public bool Validate_Type(Dictionary<string, string> _query)
        {
            var rx = new Regex(@"^\..*\z");
            string[] types = new string[] { "text", "audio", "photo", "video" };

            return rx.IsMatch(_query["type"]) || types.Any(i => i == _query["type"]);
        }

        [ParameterValidation("content")]
        public bool Validate_Content(Dictionary<string, string> _query)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(_query["content"]);

            return bytes.Length < 2048;
        }

        [ParameterValidation("messageId")]
        public bool Validate_Message(Dictionary<string, string> _query)
        {
            using var dbContext = new MessengerContext();

            var messages = dbContext.Messages.FirstOrDefault(i => i.Id == Convert.ToInt32(_query["messageId"]));

            return messages != null;
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

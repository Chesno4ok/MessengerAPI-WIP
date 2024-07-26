using System.Drawing;
using ChesnokMessengerAPI.Responses;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace ChesnokMessengerAPI.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private MessengerContext _dbContext;
        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerContext();
        }

        public Task InvokeAsync(HttpContext context)
        {
            _next.Invoke(context);

            try
            {
                
            }
            catch
            {
                context.Response.StatusCode = 500;
                context.Response.WriteAsync(new Response("Error", "Server not responding. Try again later").ToJson());
            }


            return Task.CompletedTask;
        }
    }
}
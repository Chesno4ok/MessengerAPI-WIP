using System.Drawing;
using ChesnokMessengerAPI.Responses;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace ChesnokMessengerAPI.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private MessengerApiContext _dbContext;
        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
            _dbContext = new MessengerApiContext();
        }

        public Task InvokeAsync(HttpContext context)
        {
            try
            {
                _next.Invoke(context);
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
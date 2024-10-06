namespace ChesnokMessengerAPI.Middleware
{
    public class FooMiddleware
    {
        RequestDelegate _next;
        public FooMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            
            await _next.Invoke(context);
            _next.Invoke(context);
        }
    }
}

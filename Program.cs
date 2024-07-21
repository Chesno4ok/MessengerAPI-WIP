
using ChesnokMessengerAPI.Middleware;
using ChesnokMessengerAPI.Services;

namespace ChesnokMessengerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<GCService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<ExceptionHandler>();
            app.UseMiddleware<ParamCheckMiddleware>();

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            };
            app.UseWebSockets(webSocketOptions);

            app.Run();
        }
    }
}
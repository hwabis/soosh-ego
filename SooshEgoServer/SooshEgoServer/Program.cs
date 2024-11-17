
using SooshEgoServer.GameLogic;
using SooshEgoServer.Hubs;

namespace SooshEgoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IGamesManager, GamesManager>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapHub<GameHub>($"/{nameof(GameHub)}");

            app.Run();
        }
    }
}

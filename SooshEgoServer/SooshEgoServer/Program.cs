
using SooshEgoServer.GameManagement;
using SooshEgoServer.Hubs;

namespace SooshEgoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            "http://localhost:3000" // todo ihni how to handle production
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IGamesManager, GamesManager>();
            builder.Services.AddSingleton<GameUpdateNotifier>();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapHub<GameHub>($"/game-hub");
            SubscribeGameNotifierToGamesManager(app);

            app.Run();
        }

        private static void SubscribeGameNotifierToGamesManager(WebApplication app)
        {
            IGamesManager gamesManager = app.Services.GetRequiredService<IGamesManager>();
            GameUpdateNotifier notifier = app.Services.GetRequiredService<GameUpdateNotifier>();
            gamesManager.GameStateUpdated += notifier.NotifyGameStateUpdated;
        }
    }
}

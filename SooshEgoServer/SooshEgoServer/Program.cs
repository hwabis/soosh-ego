using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SooshEgoServer.Data;
using SooshEgoServer.Hubs;
using SooshEgoServer.Services;

namespace SooshEgoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string[] allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(",") ?? [];

            if (allowedOrigins.Length > 0)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
            }

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IGamesManager, GamesManager>();
            builder.Services.AddSingleton<GameUpdateNotifier>();

            string? connectionString = builder.Configuration.GetConnectionString("CONNECTION_STRING");

            if (!connectionString.IsNullOrEmpty())
            {
                builder.Services.AddDbContext<SooshEgoDbContext>(
                    options => options.UseSqlServer(connectionString));
            }

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                if (!connectionString.IsNullOrEmpty())
                {
                    using IServiceScope scope = app.Services.CreateScope();
                    SooshEgoDbContext db = scope.ServiceProvider.GetRequiredService<SooshEgoDbContext>();
                    db.Database.EnsureCreated();
                }
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

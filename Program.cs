
using AspNet.Security.OAuth.Discord;
using GroundhogWeb.Models;
using GroundhogWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GroundhogWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<DatabaseSettings>(
            builder.Configuration.GetSection("MongoDB"));

            builder.Services.AddSingleton<GuildsService>();

            // Add authentication services
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddDiscord(options =>
            {
                options.ClientId = builder.Configuration["Discord:ClientId"];
                options.ClientSecret = builder.Configuration["Discord:ClientSecret"];
                options.CallbackPath = builder.Configuration["Discord:RedirectUri"];

                options.Scope.Add("identify");
                options.Scope.Add("email");
                options.Scope.Add("guilds");
                options.SaveTokens = true;

            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // Add this line
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
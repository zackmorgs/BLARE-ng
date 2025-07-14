using MongoDB.Driver;
using Data;
using Models;
using Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register DataContext first
        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(
            builder.Configuration.GetConnectionString("MongoConnection")
        ));
        builder.Services.AddSingleton<DataContext>();

        // Services that depend on DataContext
        builder.Services.AddSingleton<TrackService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<ReleaseService>();
        
        // JWT service only needs IConfiguration
        builder.Services.AddSingleton<JwtService>();
        
        builder.Services.AddControllers();

        // JWT Configuration
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        builder.Services.AddAuthorization();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", 
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });

        var app = builder.Build();

        // Use CORS
        app.UseCors("AllowAngularApp");

        // Enable static file serving
        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers(); // This was missing - critical for routing!

        app.Run();
    }
}
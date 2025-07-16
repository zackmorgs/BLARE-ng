using System.Text;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddSingleton<TrackService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<JwtService>();
        builder.Services.AddSingleton<ReleaseService>();
        builder.Services.AddSingleton<TagService>();
        builder.Services.AddSingleton<SlugService>();

        builder.Services.AddControllers();

        // Configure Kestrel server limits for file uploads
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 3000L * 1024 * 1024; // 3000MB
            options.ValueLengthLimit = int.MaxValue;
            options.ValueCountLimit = int.MaxValue;
            options.KeyLengthLimit = int.MaxValue;
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 3000L * 1024 * 1024; // 3000MB
        });

        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(
            builder.Configuration.GetConnectionString("MongoConnection")
        ));

        builder.Services.AddSingleton<DataContext>();

        // JWT Configuration
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");

        builder
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        // Add CORS - Allow all requests for development
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            );
        });

        var app = builder.Build();

        // Add request logging middleware
        app.Use(
            async (context, next) =>
            {
                Console.WriteLine(
                    $"=== REQUEST: {context.Request.Method} {context.Request.Path} ==="
                );
                Console.WriteLine(
                    $"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"))}"
                );
                await next();
            }
        );

        // Use CORS first - before any other middleware
        app.UseCors("AllowAll");

        // Enable static file serving for uploads
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers(); // This was missing - critical for routing!

        app.Run();
    }
}

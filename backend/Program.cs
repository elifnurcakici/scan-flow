using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Middleware;
using backend.Services;
using backend.Services.Interfaces;
using backend.Services.Kafka;
using backend.Services.WebSockets;
using backend.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddSingleton<WebSocketNotificationService>();
builder.Services.AddSingleton<ScanWebSocketHandler>();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        var redisConnection = builder.Configuration["Redis:ConnectionString"];
        return ConnectionMultiplexer.Connect(redisConnection!);
    });

    builder.Services.AddScoped<IRedisTokenBlacklistService, RedisTokenBlacklistService>();
}
else
{
    builder.Services.AddScoped<IRedisTokenBlacklistService, FakeRedisTokenBlacklistService>();
}

builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IScanService, ScanService>();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddHostedService<KafkaTopicInitializerService>();
    builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
    builder.Services.AddHostedService<KafkaConsumerService>();
}
else
{
    builder.Services.AddSingleton<IKafkaProducerService, FakeKafkaProducerService>();
}

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAssetRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenVersionClaim = context.Principal?.FindFirst("tokenVersion")?.Value;
                var jtiClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId) ||
                    !int.TryParse(tokenVersionClaim, out var tokenVersion) ||
                    string.IsNullOrWhiteSpace(jtiClaim))
                {
                    context.Fail("Invalid token claims.");
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var blacklistService = context.HttpContext.RequestServices
                    .GetRequiredService<IRedisTokenBlacklistService>();
                var user = await dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == userId);

                if (user is null || user.TokenVersion != tokenVersion)
                {
                    context.Fail("Token is no longer valid.");
                    return;
                }

                var isBlacklisted = await blacklistService.IsBlacklistedAsync(jtiClaim);
                if (isBlacklisted)
                {
                    context.Fail("Token is blacklisted.");
                }
            }
        };
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = ApiResponse<object>.FailResponse(
            "Validation failed",
            errors,
            context.HttpContext.TraceIdentifier
        );

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

var app = builder.Build();

if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    await AppDbSeeder.SeedAsync(dbContext);
}

#region Middleware

app.UseCors("AllowAll");
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsEnvironment("Test"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

#endregion

#region Endpoints

app.Map("/ws/scans", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var handler = context.RequestServices.GetRequiredService<ScanWebSocketHandler>();
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await handler.HandleAsync(context, socket);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.MapControllers();

#endregion

app.Run();

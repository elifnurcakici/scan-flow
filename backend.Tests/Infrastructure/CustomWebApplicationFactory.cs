using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using backend.Data;
using backend.Services.Interfaces;
using backend.Services;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace backend.Tests.Infrastructure;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));
            services.RemoveAll(typeof(IPasswordService));
            services.RemoveAll(typeof(IRedisTokenBlacklistService));
            services.RemoveAll(typeof(IKafkaProducerService));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddScoped<IPasswordService, FakePasswordService>();
            services.AddScoped<IRedisTokenBlacklistService, FakeRedisTokenBlacklistService>();
            services.AddSingleton<IKafkaProducerService, FakeKafkaProducerService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName,
                _ => { });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            AppDbSeeder.SeedAsync(db).GetAwaiter().GetResult();
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-key-that-should-be-long-enough-12345",
                ["Jwt:Issuer"] = "ScanFlow",
                ["Jwt:Audience"] = "ScanFlowUsers",
                ["Jwt:ExpiryMinutes"] = "60",
                ["Kafka:DastScanTopic"] = "scan-dast-test",
                ["Kafka:ScaScanTopic"] = "scan-sca-test",
                ["Kafka:ScanResultTopic"] = "scan-results-test"
            };

            config.AddInMemoryCollection(dict);
        });
    }
}

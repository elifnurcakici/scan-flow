using backend.Tests.Infrastructure;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using backend.Data;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests;

public class TestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    public TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<(string accessToken, string refreshToken, string email)> RegisterAndLogin()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var password = "123456";

        var register = await Client.PostAsync("/api/auth/register",
            TestHelper.ToJson(new { email, password }));

        var registerJson = await register.Content.ReadAsStringAsync();
        Console.WriteLine("REGISTER:");
        Console.WriteLine(registerJson);

        register.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await Client.PostAsync("/api/auth/login",
            TestHelper.ToJson(new { email, password }));

        var json = await login.Content.ReadAsStringAsync();
        Console.WriteLine("LOGIN:");
        Console.WriteLine(json);

        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = JObject.Parse(json);
        var data = payload["data"]!;

        return (
            data["accessToken"]!.Value<string>()!,
            data["refreshToken"]!.Value<string>()!,
            email
        );
    }

    protected async Task<string> GetToken()
    {
        var tokens = await RegisterAndLogin();
        return tokens.accessToken;
    }

    protected async Task<(Guid userId, string email)> RegisterAndAuthenticateTestUser()
    {
        var tokens = await RegisterAndLogin();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = db.Users.Single(x => x.Email == tokens.email);

        TestHelper.SetTestAuth(Client, user.Id, user.Email);
        return (user.Id, user.Email);
    }
}

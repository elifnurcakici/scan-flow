using FluentAssertions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using backend.Tests.Infrastructure;
using backend.Tests;
using backend.Data;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Auth;
public class AuthFlowTests : TestBase
{
    public AuthFlowTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Full_Auth_Flow_Should_Work()
    {
        var (accessToken, refreshToken, email) = await RegisterAndLogin();
        accessToken.Should().NotBeNullOrWhiteSpace();
        refreshToken.Should().NotBeNullOrWhiteSpace();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = db.Users.Single(x => x.Email == email);

        TestHelper.SetTestAuth(Client, user.Id, user.Email);

        // ME
        var me = await Client.GetAsync("/api/auth/me");
        me.StatusCode.Should().Be(HttpStatusCode.OK);

        // REFRESH
        var refresh = await Client.PostAsync("/api/auth/refresh",
            TestHelper.ToJson(new { email, refreshToken }));

        refresh.StatusCode.Should().Be(HttpStatusCode.OK);

        // LOGOUT
        var logout = await Client.PostAsync("/api/auth/logout",
            TestHelper.ToJson(new { }));

        logout.StatusCode.Should().Be(HttpStatusCode.OK);

        // REFRESH AFTER LOGOUT → FAIL
        var refreshAfterLogout = await Client.PostAsync("/api/auth/refresh",
            TestHelper.ToJson(new { email, refreshToken }));

        refreshAfterLogout.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task User_Update_Should_Invalidate_Old_Refresh_And_Increment_TokenVersion()
    {
        var (_, refreshToken, email) = await RegisterAndLogin();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = db.Users.Single(x => x.Email == email);
        var originalTokenVersion = user.TokenVersion;

        TestHelper.SetTestAuth(Client, user.Id, user.Email);

        var updateResponse = await Client.PutAsync("/api/auth/update",
            TestHelper.ToJson(new
            {
                email = "updated@test.com",
                password = "654321"
            }));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        db.ChangeTracker.Clear();
        var updatedUser = db.Users.Single(x => x.Id == user.Id);

        updatedUser.Email.Should().Be("updated@test.com");
        updatedUser.TokenVersion.Should().Be(originalTokenVersion + 1);
        updatedUser.RefreshTokenHash.Should().BeNull();
        updatedUser.RefreshTokenExpiresAt.Should().BeNull();

        var refreshAfterUpdate = await Client.PostAsync("/api/auth/refresh",
            TestHelper.ToJson(new { email, refreshToken }));

        refreshAfterUpdate.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

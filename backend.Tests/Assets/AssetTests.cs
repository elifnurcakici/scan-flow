using FluentAssertions;
using System.Net;
using Newtonsoft.Json.Linq;
using backend.Tests.Infrastructure;
using backend.Data;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Assets;
public class AssetTests : TestBase
{
    public AssetTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Asset_CRUD_Should_Work()
    {
        await RegisterAndAuthenticateTestUser();

        // CREATE
        var create = await Client.PostAsync("/api/assets",
            TestHelper.ToJson(new
            {
                name = "Test Asset",
                domain = "test.com",
                type = 1
            }));

        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await create.Content.ReadAsStringAsync();
        var payload = JObject.Parse(json);
        var data = payload["data"]!;
        var assetId = data["id"]!.Value<string>()!;

        // GET BY ID
        var get = await Client.GetAsync($"/api/assets/{assetId}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = await Client.PutAsync($"/api/assets/{assetId}",
            TestHelper.ToJson(new
            {
                name = "Updated Asset",
                domain = "updated.com",
                type = 3
            }));

        update.StatusCode.Should().Be(HttpStatusCode.OK);

        // DELETE
        var delete = await Client.DeleteAsync($"/api/assets/{assetId}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // GET AGAIN → 404
        var getAgain = await Client.GetAsync($"/api/assets/{assetId}");
        getAgain.StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Assets.Any(x => x.Id == Guid.Parse(assetId)).Should().BeFalse();
    }

    [Fact]
    public async Task Asset_Create_Should_Reject_Duplicates()
    {
        await RegisterAndAuthenticateTestUser();

        var payload = new
        {
            name = "Customer Portal",
            domain = "example.com",
            type = 1
        };

        var firstCreate = await Client.PostAsync("/api/assets", TestHelper.ToJson(payload));
        firstCreate.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicateCreate = await Client.PostAsync("/api/assets", TestHelper.ToJson(payload));
        duplicateCreate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

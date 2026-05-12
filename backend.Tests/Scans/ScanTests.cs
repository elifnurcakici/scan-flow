using FluentAssertions;
using System.Net;
using backend.Tests.Infrastructure;
using backend.Tests;
using Newtonsoft.Json.Linq;
using backend.Data;
using backend.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Scans;
public class ScanTests : TestBase
{
    public ScanTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Scan_Start_Should_Create_Pending_Scan()
    {
        await RegisterAndAuthenticateTestUser();

        // create asset
        var asset = await Client.PostAsync("/api/assets",
            TestHelper.ToJson(new
            {
                name = "Scan Asset",
                domain = "scan.com",
                type = 1
            }));

        var assetJson = await asset.Content.ReadAsStringAsync();
        var assetPayload = JObject.Parse(assetJson);
        var assetData = assetPayload["data"]!;
        var assetId = assetData["id"]!.Value<string>()!;

        // start scan
        var scan = await Client.PostAsync("/api/scans/start",
            TestHelper.ToJson(new
            {
                name = "Test Scan",
                assetId
            }));

        scan.StatusCode.Should().Be(HttpStatusCode.OK);

        var scanJson = await scan.Content.ReadAsStringAsync();
        var scanPayload = JObject.Parse(scanJson);
        var scanData = scanPayload["data"]!;
        var scanId = scanData["scanId"]!.Value<string>()!;

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persistedScan = db.Scans.Single(x => x.Id == Guid.Parse(scanId));
        var history = db.ScanHistories.Single(x => x.ScanId == Guid.Parse(scanId));

        persistedScan.Status.Should().Be(ScanStatus.Pending);
        history.Status.Should().Be(ScanStatus.Pending);
    }
}

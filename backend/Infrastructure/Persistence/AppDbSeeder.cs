using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Scanners.AnyAsync())
        {
            return;
        }

        context.Scanners.AddRange(
            new Scanner
            {
                Name = "Nuclei DAST Scanner",
                Type = ScannerType.Dast,
                AssetType = AssetType.Domain,
                TopicName = "scan-dast",
                Description = "Scans exposed domains with a DAST-oriented profile."
            },
            new Scanner
            {
                Name = "Web App DAST Scanner",
                Type = ScannerType.Dast,
                AssetType = AssetType.WebApp,
                TopicName = "scan-dast",
                Description = "Scans web applications for runtime issues."
            },
            new Scanner
            {
                Name = "Repository SCA Scanner",
                Type = ScannerType.Sca,
                AssetType = AssetType.Repository,
                TopicName = "scan-sca",
                Description = "Inspects dependency metadata for vulnerable components."
            },
            new Scanner
            {
                Name = "Infrastructure Surface Scanner",
                Type = ScannerType.Dast,
                AssetType = AssetType.Ip,
                TopicName = "scan-dast",
                Description = "Runs a host-focused baseline scan against infrastructure assets."
            }
        );

        await context.SaveChangesAsync();
    }
}

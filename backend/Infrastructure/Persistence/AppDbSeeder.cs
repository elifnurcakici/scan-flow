using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var desiredScanners = new[]
        {
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
                Name = "Source Control SAST Scanner",
                Type = ScannerType.Sast,
                AssetType = AssetType.SourceControl,
                TopicName = "scan-sast",
                Description = "Runs a baseline static analysis profile against source-control assets."
            },
            new Scanner
            {
                Name = "Source Control Secret Scanner",
                Type = ScannerType.SecretScan,
                AssetType = AssetType.SourceControl,
                TopicName = "scan-secretscan",
                Description = "Looks for exposed secrets in source-control assets."
            },
            new Scanner
            {
                Name = "Source Control SCA Scanner",
                Type = ScannerType.Sca,
                AssetType = AssetType.SourceControl,
                TopicName = "scan-sca",
                Description = "Inspects dependency metadata in source-control assets for vulnerable components."
            },
            new Scanner
            {
                Name = "Container Image SCA Scanner",
                Type = ScannerType.Sca,
                AssetType = AssetType.Image,
                TopicName = "scan-sca",
                Description = "Inspects container images for vulnerable packages and misconfigurations."
            },
            new Scanner
            {
                Name = "Infrastructure IP Scanner",
                Type = ScannerType.Infra,
                AssetType = AssetType.Ip,
                TopicName = "scan-infra",
                Description = "Runs a host-focused baseline scan against infrastructure assets."
            },
            new Scanner
            {
                Name = "Cluster Infrastructure Scanner",
                Type = ScannerType.Infra,
                AssetType = AssetType.Cluster,
                TopicName = "scan-infra",
                Description = "Runs baseline checks against cluster-exposed endpoints."
            },
            new Scanner
            {
                Name = "Cloud Exposure Scanner",
                Type = ScannerType.Cloud,
                AssetType = AssetType.Cloud,
                TopicName = "scan-cloud",
                Description = "Runs a cloud-oriented baseline exposure scan."
            }
        };

        var existingScanners = await context.Scanners.ToListAsync();

        foreach (var existingScanner in existingScanners)
        {
            if (!desiredScanners.Any(x => x.Name == existingScanner.Name))
            {
                existingScanner.IsEnabled = false;
            }
        }

        foreach (var desiredScanner in desiredScanners)
        {
            var existingScanner = existingScanners.FirstOrDefault(x => x.Name == desiredScanner.Name);

            if (existingScanner is null)
            {
                context.Scanners.Add(desiredScanner);
                continue;
            }

            existingScanner.Type = desiredScanner.Type;
            existingScanner.AssetType = desiredScanner.AssetType;
            existingScanner.TopicName = desiredScanner.TopicName;
            existingScanner.Description = desiredScanner.Description;
            existingScanner.IsEnabled = true;
        }

        await context.SaveChangesAsync();
    }
}

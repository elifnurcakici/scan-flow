using Microsoft.EntityFrameworkCore;
using backend.Entities;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<Vulnerability> Vulnerabilities => Set<Vulnerability>();
    public DbSet<ScanHistory> ScanHistories => Set<ScanHistory>();
    public DbSet<Scanner> Scanners => Set<Scanner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.TokenVersion).IsRequired();
            entity.Property(x => x.RefreshTokenHash).HasMaxLength(255);
            entity.Property(x => x.RefreshTokenExpiresAt);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Asset");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Domain).IsRequired().HasMaxLength(255);
            entity.Property(x => x.NormalizedDomain).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasOne(x => x.User)
                  .WithMany(x => x.Assets)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => new { x.UserId, x.NormalizedDomain, x.Type }).IsUnique();
        });

        modelBuilder.Entity<Scan>(entity =>
        {
            entity.ToTable("Scan");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ErrorReason).HasMaxLength(500);

            entity.HasOne(x => x.Asset)
                .WithMany(x => x.Scans)
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Scanner)
                .WithMany(x => x.Scans)
                .HasForeignKey(x => x.ScannerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vulnerability>(entity =>
        {
            entity.ToTable("Vulnerability");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Severity).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Type).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(500);
            entity.Property(x => x.CweId).HasMaxLength(50);
            entity.Property(x => x.CvssScore).HasColumnType("numeric(4,1)");
            entity.Property(x => x.CvssVector).HasMaxLength(128);
            entity.Property(x => x.Recommendation).HasMaxLength(1000);

            entity.HasOne(x => x.Scan)
                .WithMany(x => x.Vulnerabilities)
                .HasForeignKey(x => x.ScanId)
                .OnDelete(DeleteBehavior.Cascade);        
        });

        modelBuilder.Entity<ScanHistory>(entity =>
        {
            entity.ToTable("ScanHistory");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).IsRequired();

            entity.HasOne(x => x.Scan)
                .WithMany(x => x.ScanHistories)
                .HasForeignKey(x => x.ScanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Scanner>(entity =>
        {
            entity.ToTable("Scanner");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.AssetType).IsRequired();
            entity.Property(x => x.TopicName).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(400);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.IsEnabled).IsRequired();

            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasIndex(x => new { x.AssetType, x.Type, x.IsEnabled });
        });
    }
}

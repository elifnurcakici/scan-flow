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
            entity.Property(x => x.Type).IsRequired().HasMaxLength(50);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasOne(x => x.User)
                  .WithMany(x => x.Assets)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => new { x.UserId, x.Domain});
        });

        modelBuilder.Entity<Scan>(entity =>
        {
            entity.ToTable("Scan");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ErrorReason).HasMaxLength(500);

            entity.HasOne(x => x.Asset)
                .WithMany()
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Vulnerability>(entity =>
        {
            entity.ToTable("Vulnerability");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Severity).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Type).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(500);

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
    }
}

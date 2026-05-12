using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSecurityModelAndScannerRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Asset_UserId_Domain\";");

            migrationBuilder.CreateTable(
                name: "Scanner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    TopicName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scanner", x => x.Id);
                });

            migrationBuilder.Sql("""
                INSERT INTO "Scanner" ("Id", "Name", "Type", "AssetType", "TopicName", "IsEnabled", "Description", "CreatedAt")
                VALUES
                ('11111111-1111-1111-1111-111111111111', 'Nuclei DAST Scanner', 1, 1, 'scan-dast', TRUE, 'Scans exposed domains with a DAST-oriented profile.', NOW()),
                ('22222222-2222-2222-2222-222222222222', 'Infrastructure Surface Scanner', 1, 2, 'scan-dast', TRUE, 'Runs a host-focused baseline scan against infrastructure assets.', NOW()),
                ('33333333-3333-3333-3333-333333333333', 'Web App DAST Scanner', 1, 3, 'scan-dast', TRUE, 'Scans web applications for runtime issues.', NOW()),
                ('44444444-4444-4444-4444-444444444444', 'Repository SCA Scanner', 3, 4, 'scan-sca', TRUE, 'Inspects dependency metadata for vulnerable components.', NOW());
                """);

            migrationBuilder.Sql("""
                CREATE TEMP TABLE tmp_user_map AS
                SELECT "Id" AS old_id, gen_random_uuid() AS new_id
                FROM "User";

                CREATE TEMP TABLE tmp_asset_map AS
                SELECT "Id" AS old_id, gen_random_uuid() AS new_id
                FROM "Asset";

                CREATE TEMP TABLE tmp_scan_map AS
                SELECT "Id" AS old_id, gen_random_uuid() AS new_id
                FROM "Scan";

                CREATE TEMP TABLE tmp_asset_canonical AS
                SELECT a."Id" AS old_id,
                       first_value(am.new_id) OVER (
                           PARTITION BY a."UserId", lower(trim(a."Domain")), a."Type"
                           ORDER BY a."CreatedAt", a."Id"
                       ) AS new_id,
                       first_value(a."Id") OVER (
                           PARTITION BY a."UserId", lower(trim(a."Domain")), a."Type"
                           ORDER BY a."CreatedAt", a."Id"
                       ) AS canonical_old_id
                FROM "Asset" a
                JOIN tmp_asset_map am ON am.old_id = a."Id";
                """);

            migrationBuilder.CreateTable(
                name: "User_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TokenVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_New", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asset_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedDomain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_New", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_New_User_New_UserId",
                        column: x => x.UserId,
                        principalTable: "User_New",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scan_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScannerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scan_New", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scan_New_Asset_New_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset_New",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scan_New_Scanner_ScannerId",
                        column: x => x.ScannerId,
                        principalTable: "Scanner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScanHistory_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanHistory_New", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScanHistory_New_Scan_New_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scan_New",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vulnerability_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CweId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CvssScore = table.Column<decimal>(type: "numeric(4,1)", nullable: true),
                    CvssVector = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Recommendation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerability_New", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vulnerability_New_Scan_New_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scan_New",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO "User_New" ("Id", "Email", "PasswordHash", "CreatedAt", "UpdatedAt", "RefreshTokenHash", "RefreshTokenExpiresAt", "TokenVersion")
                SELECT map.new_id,
                       u."Email",
                       u."PasswordHash",
                       u."CreatedAt",
                       COALESCE(u."UpdatedAt", u."CreatedAt"),
                       u."RefreshTokenHash",
                       u."RefreshTokenExpiresAt",
                       u."TokenVersion"
                FROM "User" u
                JOIN tmp_user_map map ON map.old_id = u."Id";

                INSERT INTO "Asset_New" ("Id", "Name", "Domain", "NormalizedDomain", "Type", "CreatedAt", "UpdatedAt", "UserId")
                SELECT ac.new_id,
                       a."Name",
                       a."Domain",
                       lower(trim(a."Domain")),
                       a."Type",
                       a."CreatedAt",
                       COALESCE(a."UpdatedAt", a."CreatedAt"),
                       umap.new_id
                FROM "Asset" a
                JOIN tmp_asset_canonical ac ON ac.old_id = a."Id"
                JOIN tmp_user_map umap ON umap.old_id = a."UserId"
                WHERE ac.old_id = ac.canonical_old_id;

                INSERT INTO "Scan_New" ("Id", "Name", "AssetId", "ScannerId", "Status", "ErrorReason", "CreatedAt")
                SELECT smap.new_id,
                       s."Name",
                       ac.new_id,
                       CASE a."Type"
                           WHEN 1 THEN '11111111-1111-1111-1111-111111111111'::uuid
                           WHEN 2 THEN '22222222-2222-2222-2222-222222222222'::uuid
                           WHEN 3 THEN '33333333-3333-3333-3333-333333333333'::uuid
                           WHEN 4 THEN '44444444-4444-4444-4444-444444444444'::uuid
                           ELSE '11111111-1111-1111-1111-111111111111'::uuid
                       END,
                       s."Status",
                       s."ErrorReason",
                       s."CreatedAt"
                FROM "Scan" s
                JOIN "Asset" a ON a."Id" = s."AssetId"
                JOIN tmp_scan_map smap ON smap.old_id = s."Id"
                JOIN tmp_asset_canonical ac ON ac.old_id = s."AssetId";

                INSERT INTO "ScanHistory_New" ("Id", "ScanId", "Status", "CreatedAt")
                SELECT gen_random_uuid(),
                       smap.new_id,
                       sh."Status",
                       sh."CreatedAt"
                FROM "ScanHistory" sh
                JOIN tmp_scan_map smap ON smap.old_id = sh."ScanId";

                INSERT INTO "Vulnerability_New" ("Id", "ScanId", "Severity", "Type", "Description", "CweId", "CvssScore", "CvssVector", "Recommendation", "CreatedAt")
                SELECT gen_random_uuid(),
                       smap.new_id,
                       v."Severity",
                       v."Type",
                       v."Description",
                       NULL,
                       NULL,
                       NULL,
                       NULL,
                       v."CreatedAt"
                FROM "Vulnerability" v
                JOIN tmp_scan_map smap ON smap.old_id = v."ScanId";
                """);

            migrationBuilder.DropTable(name: "Vulnerability");
            migrationBuilder.DropTable(name: "ScanHistory");
            migrationBuilder.DropTable(name: "Scan");
            migrationBuilder.DropTable(name: "Asset");
            migrationBuilder.DropTable(name: "User");

            migrationBuilder.RenameTable(name: "User_New", newName: "User");
            migrationBuilder.RenameTable(name: "Asset_New", newName: "Asset");
            migrationBuilder.RenameTable(name: "Scan_New", newName: "Scan");
            migrationBuilder.RenameTable(name: "ScanHistory_New", newName: "ScanHistory");
            migrationBuilder.RenameTable(name: "Vulnerability_New", newName: "Vulnerability");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_UserId_NormalizedDomain_Type",
                table: "Asset",
                columns: new[] { "UserId", "NormalizedDomain", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_UserId",
                table: "Asset",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scan_AssetId",
                table: "Scan",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Scan_ScannerId",
                table: "Scan",
                column: "ScannerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanHistory_ScanId",
                table: "ScanHistory",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerability_ScanId",
                table: "Vulnerability",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Scanner_AssetType_Type_IsEnabled",
                table: "Scanner",
                columns: new[] { "AssetType", "Type", "IsEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_Scanner_Name",
                table: "Scanner",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Scanner\";");
        }
    }
}

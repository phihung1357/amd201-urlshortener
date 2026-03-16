using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_AddRedirectLogs_Stats_UpdateDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomAlias",
                table: "ShortUrls",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RedirectLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedirectLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedirectLogs_ShortUrls_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "ShortUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RedirectLogs_ShortUrlId",
                table: "RedirectLogs",
                column: "ShortUrlId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RedirectLogs");

            migrationBuilder.DropColumn(
                name: "CustomAlias",
                table: "ShortUrls");
        }
    }
}

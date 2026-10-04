using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _03102026_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BondLifePortfolioPositionEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SevenEtfLifePortfolioPositionEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ThreeEtfLifePortfolioPositionEntities",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "BondLifePositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BondLifePositionEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SevenEtfLifePositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SevenEtfLifePositionEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShareLifePositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareLifePositionEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreeEtfLifePositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreeEtfLifePositionEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BondLifePositionEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SevenEtfLifePositionEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ShareLifePositionEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ThreeEtfLifePositionEntities",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "BondLifePortfolioPositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BondLifePortfolioPositionEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SevenEtfLifePortfolioPositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SevenEtfLifePortfolioPositionEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreeEtfLifePortfolioPositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreeEtfLifePortfolioPositionEntities", x => x.Id);
                });
        }
    }
}

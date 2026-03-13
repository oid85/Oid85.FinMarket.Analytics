using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _13032026_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LifePortfolioPositionEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifePortfolioPositionEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifePortfolioPositionEntities",
                schema: "public");
        }
    }
}

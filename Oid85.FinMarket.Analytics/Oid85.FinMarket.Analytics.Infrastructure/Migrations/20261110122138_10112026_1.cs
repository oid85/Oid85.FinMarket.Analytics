using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _10112026_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InPortfolio",
                schema: "public",
                table: "InstrumentEntities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InPortfolio",
                schema: "public",
                table: "InstrumentEntities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

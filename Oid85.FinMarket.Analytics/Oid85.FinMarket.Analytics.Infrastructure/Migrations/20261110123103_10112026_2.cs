using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _10112026_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualCoefficient",
                schema: "public",
                table: "InstrumentEntities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ManualCoefficient",
                schema: "public",
                table: "InstrumentEntities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

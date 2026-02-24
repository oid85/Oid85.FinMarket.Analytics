using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _24022026_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DividendCoefficient",
                schema: "public",
                table: "InstrumentEntities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ManualCoefficient",
                schema: "public",
                table: "InstrumentEntities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DividendCoefficient",
                schema: "public",
                table: "InstrumentEntities");

            migrationBuilder.DropColumn(
                name: "ManualCoefficient",
                schema: "public",
                table: "InstrumentEntities");
        }
    }
}

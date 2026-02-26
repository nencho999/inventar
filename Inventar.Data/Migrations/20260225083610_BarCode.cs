using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class BarCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarCode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "Products");
        }
    }
}

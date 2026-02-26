using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSalesPointToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesPointProducts_Materials_ProductId",
                table: "SalesPointProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesPointProducts_Products_ProductId",
                table: "SalesPointProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesPointProducts_Products_ProductId",
                table: "SalesPointProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesPointProducts_Materials_ProductId",
                table: "SalesPointProducts",
                column: "ProductId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

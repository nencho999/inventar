using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCenterExpense_ProductionCenters_ProductionCenterId",
                table: "ProductionCenterExpense");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCenterExpense",
                table: "ProductionCenterExpense");

            migrationBuilder.RenameTable(
                name: "ProductionCenterExpense",
                newName: "ProductionCenterExpenses");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionCenterExpense_ProductionCenterId",
                table: "ProductionCenterExpenses",
                newName: "IX_ProductionCenterExpenses_ProductionCenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCenterExpenses",
                table: "ProductionCenterExpenses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCenterExpenses_ProductionCenters_ProductionCenterId",
                table: "ProductionCenterExpenses",
                column: "ProductionCenterId",
                principalTable: "ProductionCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCenterExpenses_ProductionCenters_ProductionCenterId",
                table: "ProductionCenterExpenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCenterExpenses",
                table: "ProductionCenterExpenses");

            migrationBuilder.RenameTable(
                name: "ProductionCenterExpenses",
                newName: "ProductionCenterExpense");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionCenterExpenses_ProductionCenterId",
                table: "ProductionCenterExpense",
                newName: "IX_ProductionCenterExpense_ProductionCenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCenterExpense",
                table: "ProductionCenterExpense",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCenterExpense_ProductionCenters_ProductionCenterId",
                table: "ProductionCenterExpense",
                column: "ProductionCenterId",
                principalTable: "ProductionCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

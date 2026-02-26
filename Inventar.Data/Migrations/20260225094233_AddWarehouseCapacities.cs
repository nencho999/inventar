using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseCapacities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capacities_PrimaryMaterialBases_PrimaryMaterialBaseId",
                table: "Capacities");

            migrationBuilder.DropIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryMaterialBaseId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities",
                columns: new[] { "PrimaryMaterialBaseId", "MaterialId" },
                unique: true,
                filter: "[PrimaryMaterialBaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_WarehouseId",
                table: "Capacities",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Capacities_PrimaryMaterialBases_PrimaryMaterialBaseId",
                table: "Capacities",
                column: "PrimaryMaterialBaseId",
                principalTable: "PrimaryMaterialBases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Capacities_Warehouses_WarehouseId",
                table: "Capacities",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capacities_PrimaryMaterialBases_PrimaryMaterialBaseId",
                table: "Capacities");

            migrationBuilder.DropForeignKey(
                name: "FK_Capacities_Warehouses_WarehouseId",
                table: "Capacities");

            migrationBuilder.DropIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities");

            migrationBuilder.DropIndex(
                name: "IX_Capacities_WarehouseId",
                table: "Capacities");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Capacities");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryMaterialBaseId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities",
                columns: new[] { "PrimaryMaterialBaseId", "MaterialId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Capacities_PrimaryMaterialBases_PrimaryMaterialBaseId",
                table: "Capacities",
                column: "PrimaryMaterialBaseId",
                principalTable: "PrimaryMaterialBases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class procuts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCenterStorages_Materials_MaterialId",
                table: "ProductionCenterStorages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCenterStorages",
                table: "ProductionCenterStorages");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCenterStorages_ProductionCenterId",
                table: "ProductionCenterStorages");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "ProductionCenterStorages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "ProductionCenterStorages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCenterStorages",
                table: "ProductionCenterStorages",
                columns: new[] { "ProductionCenterId", "ProductId" });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCenterStorages_ProductId",
                table: "ProductionCenterStorages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCenterStorages_Materials_MaterialId",
                table: "ProductionCenterStorages",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCenterStorages_Products_ProductId",
                table: "ProductionCenterStorages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCenterStorages_Materials_MaterialId",
                table: "ProductionCenterStorages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCenterStorages_Products_ProductId",
                table: "ProductionCenterStorages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCenterStorages",
                table: "ProductionCenterStorages");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCenterStorages_ProductId",
                table: "ProductionCenterStorages");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductionCenterStorages");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "ProductionCenterStorages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCenterStorages",
                table: "ProductionCenterStorages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCenterStorages_ProductionCenterId",
                table: "ProductionCenterStorages",
                column: "ProductionCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCenterStorages_Materials_MaterialId",
                table: "ProductionCenterStorages",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesPointExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: true),
                    CustomIntervalCount = table.Column<int>(type: "int", nullable: true),
                    OneTimeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SalesPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPointExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesPointExpense_SalesPoints_SalesPointId",
                        column: x => x.SalesPointId,
                        principalTable: "SalesPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesPointProducts",
                columns: table => new
                {
                    SalesPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    PriceReductionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPointProducts", x => new { x.SalesPointId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_SalesPointProducts_Materials_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesPointProducts_SalesPoints_SalesPointId",
                        column: x => x.SalesPointId,
                        principalTable: "SalesPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPointExpense_SalesPointId",
                table: "SalesPointExpense",
                column: "SalesPointId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPointProducts_ProductId",
                table: "SalesPointProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesPointExpense");

            migrationBuilder.DropTable(
                name: "SalesPointProducts");

            migrationBuilder.DropTable(
                name: "SalesPoints");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventar.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId",
                table: "Capacities");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Capacities");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Capacities");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Capacities",
                newName: "CapacityLimit");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCreatedByAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_PrimaryMaterialBases_BaseId",
                        column: x => x.BaseId,
                        principalTable: "PrimaryMaterialBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    IntervalMonths = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringExpenses_PrimaryMaterialBases_BaseId",
                        column: x => x.BaseId,
                        principalTable: "PrimaryMaterialBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_PrimaryMaterialBases_BaseId",
                        column: x => x.BaseId,
                        principalTable: "PrimaryMaterialBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_MaterialId",
                table: "Capacities",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities",
                columns: new[] { "PrimaryMaterialBaseId", "MaterialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BaseId",
                table: "Expenses",
                column: "BaseId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringExpenses_BaseId",
                table: "RecurringExpenses",
                column: "BaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_BaseId",
                table: "StockTransactions",
                column: "BaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_MaterialId",
                table: "StockTransactions",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Capacities_Materials_MaterialId",
                table: "Capacities",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capacities_Materials_MaterialId",
                table: "Capacities");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "RecurringExpenses");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Capacities_MaterialId",
                table: "Capacities");

            migrationBuilder.DropIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId_MaterialId",
                table: "Capacities");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Capacities");

            migrationBuilder.RenameColumn(
                name: "CapacityLimit",
                table: "Capacities",
                newName: "Quantity");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Capacities_PrimaryMaterialBaseId",
                table: "Capacities",
                column: "PrimaryMaterialBaseId");
        }
    }
}

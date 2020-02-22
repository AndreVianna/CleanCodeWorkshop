using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XPenC.DataAccess.EntityFrameworkCore.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    MealTotal = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(8,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseReportItems",
                columns: table => new
                {
                    ExpenseReportId = table.Column<int>(nullable: false),
                    ItemNumber = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    ExpenseType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(8,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseReportItems", x => new { x.ExpenseReportId, x.ItemNumber });
                    table.ForeignKey(
                        name: "FK_ExpenseReportItems_ExpenseReports_ExpenseReportId",
                        column: x => x.ExpenseReportId,
                        principalTable: "ExpenseReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseReportItems");

            migrationBuilder.DropTable(
                name: "ExpenseReports");
        }
    }
}

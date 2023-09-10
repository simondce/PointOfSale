using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSKCrackers.Migrations
{
    public partial class psk2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceivedItems");

            migrationBuilder.DropTable(
                name: "GoodsReceived");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsReceived",
                columns: table => new
                {
                    GoodsReceivedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalReceivedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceived", x => x.GoodsReceivedId);
                    table.ForeignKey(
                        name: "FK_GoodsReceived_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceivedItems",
                columns: table => new
                {
                    GoodsReceivedItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceivedId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    QuantityReceived = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceivedItems", x => x.GoodsReceivedItemId);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedItems_GoodsReceived_GoodsReceivedId",
                        column: x => x.GoodsReceivedId,
                        principalTable: "GoodsReceived",
                        principalColumn: "GoodsReceivedId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceived_SupplierId",
                table: "GoodsReceived",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedItems_GoodsReceivedId",
                table: "GoodsReceivedItems",
                column: "GoodsReceivedId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedItems_ProductId",
                table: "GoodsReceivedItems",
                column: "ProductId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSKCrackers.Migrations
{
    public partial class psk6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "SaleItems");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "SaleItems",
                newName: "QuantityInCart");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedTotal",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedTotal",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "QuantityInCart",
                table: "SaleItems",
                newName: "Quantity");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "SaleItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

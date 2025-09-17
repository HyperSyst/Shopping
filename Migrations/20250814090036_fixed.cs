using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProdcutId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProdcutId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProdcutId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "OrderDetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProdcutId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProdcutId",
                table: "OrderDetails",
                column: "ProdcutId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProdcutId",
                table: "OrderDetails",
                column: "ProdcutId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping.Migrations
{
    /// <inheritdoc />
    public partial class editnew1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId1",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ProductId1",
                table: "OrderDetails",
                newName: "ProdcutId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId1",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProdcutId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProdcutId",
                table: "OrderDetails",
                column: "ProdcutId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProdcutId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ProdcutId",
                table: "OrderDetails",
                newName: "ProductId1");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProdcutId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId1",
                table: "OrderDetails",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}

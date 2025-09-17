using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping.Migrations
{
    /// <inheritdoc />
    public partial class ratingupd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rating_ProductID",
                table: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ProductID",
                table: "Rating",
                column: "ProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rating_ProductID",
                table: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ProductID",
                table: "Rating",
                column: "ProductID",
                unique: true);
        }
    }
}

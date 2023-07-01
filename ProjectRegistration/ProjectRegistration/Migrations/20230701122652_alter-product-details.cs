using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRegistration.Migrations
{
    public partial class alterproductdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Products",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_StudentId",
                table: "Products",
                newName: "IX_Products_UserId");

            migrationBuilder.RenameColumn(
                name: "ProductPath",
                table: "ProductDetails",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Info",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Products",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_UserId",
                table: "Products",
                newName: "IX_Products_StudentId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ProductDetails",
                newName: "ProductPath");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRegistration.Migrations
{
    public partial class addusertoproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductDetails",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_UserId",
                table: "ProductDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_AspNetUsers_UserId",
                table: "ProductDetails",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_AspNetUsers_UserId",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_UserId",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductDetails");
        }
    }
}

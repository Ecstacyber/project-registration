using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRegistration.Migrations
{
    /// <inheritdoc />
    public partial class Courses_Classes_Ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CYear",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassId",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Classes");

            migrationBuilder.AlterColumn<int>(
                name: "CYear",
                table: "Classes",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

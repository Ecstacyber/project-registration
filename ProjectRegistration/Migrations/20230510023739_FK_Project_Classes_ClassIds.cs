using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRegistration.Migrations
{
    /// <inheritdoc />
    public partial class FK_Project_Classes_ClassIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CourseId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CourseId2",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "CourseId2",
                table: "Projects",
                newName: "ClassId2");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Projects",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CourseId2",
                table: "Projects",
                newName: "IX_Projects_ClassId2");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CourseId",
                table: "Projects",
                newName: "IX_Projects_ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ClassId",
                table: "Projects",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ClassId2",
                table: "Projects",
                column: "ClassId2",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ClassId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ClassId2",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ClassId2",
                table: "Projects",
                newName: "CourseId2");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Projects",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ClassId2",
                table: "Projects",
                newName: "IX_Projects_CourseId2");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ClassId",
                table: "Projects",
                newName: "IX_Projects_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CourseId",
                table: "Projects",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CourseId2",
                table: "Projects",
                column: "CourseId2",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}

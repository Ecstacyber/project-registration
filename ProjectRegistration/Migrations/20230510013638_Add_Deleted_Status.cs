using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRegistration.Migrations
{
    /// <inheritdoc />
    public partial class Add_Deleted_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "StudentStats",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "StudentStats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Projects",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProjectMembers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "ProjectMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Products",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "ProductDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "LecturerStats",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "LecturerStats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Documents",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Departments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Courses",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Classes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Classes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "StudentStats");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "StudentStats");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "LecturerStats");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "LecturerStats");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Classes");
        }
    }
}

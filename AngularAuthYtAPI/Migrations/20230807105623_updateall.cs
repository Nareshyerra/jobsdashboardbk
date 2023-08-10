using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularAuthYtAPI.Migrations
{
    public partial class updateall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedDate",
                table: "Resumes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "job",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Positions",
                table: "job",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Salary",
                table: "job",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "AppliedJob",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Positions",
                table: "AppliedJob",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "AppliedJob",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Salary",
                table: "AppliedJob",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedDate",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "job");

            migrationBuilder.DropColumn(
                name: "Positions",
                table: "job");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "job");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "job");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AppliedJob");

            migrationBuilder.DropColumn(
                name: "Positions",
                table: "AppliedJob");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "AppliedJob");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "AppliedJob");
        }
    }
}

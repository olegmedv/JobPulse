using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPulse.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddJobDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "direct_apply_url",
                table: "jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "easy_apply",
                table: "jobs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "industry",
                table: "jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "job_function",
                table: "jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "job_level",
                table: "jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "job_type",
                table: "jobs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "direct_apply_url",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "easy_apply",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "industry",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "job_function",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "job_level",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "job_type",
                table: "jobs");
        }
    }
}

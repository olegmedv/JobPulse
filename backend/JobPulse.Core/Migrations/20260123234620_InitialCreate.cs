using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPulse.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    company = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    date_posted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_remote = table.Column<bool>(type: "boolean", nullable: true),
                    salary_min = table.Column<decimal>(type: "numeric", nullable: true),
                    salary_max = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPulse.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferencesAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_notified_jobs",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    job_id = table.Column<string>(type: "text", nullable: false),
                    notified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notified_jobs", x => new { x.user_id, x.job_id });
                    table.ForeignKey(
                        name: "FK_user_notified_jobs_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    keywords = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    is_remote = table.Column<bool>(type: "boolean", nullable: true),
                    filter_keywords = table.Column<string>(type: "text", nullable: true),
                    notifications_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_notified_jobs_job_id",
                table: "user_notified_jobs",
                column: "job_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_notified_jobs");

            migrationBuilder.DropTable(
                name: "user_preferences");
        }
    }
}

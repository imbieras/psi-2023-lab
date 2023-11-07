using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyBuddy.Migrations
{
    /// <inheritdoc />
    public partial class USPTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSeenProfile",
                table: "UserSeenProfile");

            migrationBuilder.RenameTable(
                name: "UserSeenProfile",
                newName: "UserSeenProfiles");

            migrationBuilder.RenameColumn(
                name: "SeenProfileId",
                table: "UserSeenProfiles",
                newName: "SeenUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSeenProfile_UserId",
                table: "UserSeenProfiles",
                newName: "IX_UserSeenProfiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSeenProfile_SeenProfileId",
                table: "UserSeenProfiles",
                newName: "IX_UserSeenProfiles_SeenUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "UserSeenProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSeenProfiles",
                table: "UserSeenProfiles",
                columns: new[] { "UserId", "SeenUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSeenProfiles",
                table: "UserSeenProfiles");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "UserSeenProfiles");

            migrationBuilder.RenameTable(
                name: "UserSeenProfiles",
                newName: "UserSeenProfile");

            migrationBuilder.RenameColumn(
                name: "SeenUserId",
                table: "UserSeenProfile",
                newName: "SeenProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSeenProfiles_UserId",
                table: "UserSeenProfile",
                newName: "IX_UserSeenProfile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSeenProfiles_SeenUserId",
                table: "UserSeenProfile",
                newName: "IX_UserSeenProfile_SeenProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSeenProfile",
                table: "UserSeenProfile",
                columns: new[] { "UserId", "SeenProfileId" });
        }
    }
}

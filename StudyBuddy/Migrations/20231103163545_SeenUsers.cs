using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyBuddy.Migrations
{
    /// <inheritdoc />
    public partial class SeenUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedIndexesArray",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Conversations",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ConversationId",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "UserSeenProfile",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SeenProfileId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSeenProfile", x => new { x.UserId, x.SeenProfileId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSeenProfile_SeenProfileId",
                table: "UserSeenProfile",
                column: "SeenProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeenProfile_UserId",
                table: "UserSeenProfile",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSeenProfile");

            migrationBuilder.AddColumn<int[]>(
                name: "UsedIndexesArray",
                table: "Users",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Conversations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

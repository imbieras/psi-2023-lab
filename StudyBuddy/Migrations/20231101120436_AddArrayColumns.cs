using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyBuddy.Migrations
{
    /// <inheritdoc />
    public partial class AddArrayColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hobbies",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UsedIndexes",
                table: "Users",
                newName: "UsedIndexesArray");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Matches",
                newName: "MatchId");

            migrationBuilder.AddColumn<string[]>(
                name: "HobbiesArray",
                table: "Users",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MatchRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequesterId = table.Column<string>(type: "text", nullable: false),
                    RequestedId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchRequests", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User1Id",
                table: "Matches",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User2Id",
                table: "Matches",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches",
                column: "User1Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches",
                column: "User2Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "MatchRequests");

            migrationBuilder.DropIndex(
                name: "IX_Matches_User1Id",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_User2Id",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HobbiesArray",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "UsedIndexesArray",
                table: "Users",
                newName: "UsedIndexes");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "Matches",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Hobbies",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

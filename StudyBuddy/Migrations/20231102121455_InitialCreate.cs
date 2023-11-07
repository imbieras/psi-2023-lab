using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyBuddy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchRequests",
                columns: table => new
                {
                    RequesterId = table.Column<string>(type: "text", nullable: false),
                    RequestedId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchRequests", x => x.RequesterId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UsedIndexesArray = table.Column<int[]>(type: "integer[]", nullable: false),
                    HobbiesArray = table.Column<string[]>(type: "text[]", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Flags = table.Column<byte>(type: "smallint", nullable: false),
                    Traits_Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Traits_Subject = table.Column<string>(type: "text", nullable: false),
                    Traits_AvatarPath = table.Column<string>(type: "text", nullable: false),
                    Traits_Description = table.Column<string>(type: "text", nullable: false),
                    Traits_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Traits_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    User1Id = table.Column<string>(type: "text", nullable: false),
                    User2Id = table.Column<string>(type: "text", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => new { x.User1Id, x.User2Id });
                    table.ForeignKey(
                        name: "FK_Matches_Users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Users_User2Id",
                        column: x => x.User2Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User1Id_User2Id",
                table: "Matches",
                columns: new[] { "User1Id", "User2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User2Id",
                table: "Matches",
                column: "User2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "MatchRequests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

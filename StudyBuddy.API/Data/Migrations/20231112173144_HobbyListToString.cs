#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyBuddy.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class HobbyListToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HobbiesArray",
                table: "Users",
                newName: "Hobbies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Hobbies",
                table: "Users",
                newName: "HobbiesArray");
        }
    }
}

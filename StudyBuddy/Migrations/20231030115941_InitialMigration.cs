using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyBuddy.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    User1Id = table.Column<string>(type: "text", nullable: false),
                    User2Id = table.Column<string>(type: "text", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UsedIndexes = table.Column<List<int>>(type: "integer[]", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Flags = table.Column<byte>(type: "smallint", nullable: false),
                    Traits_Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Traits_Subject = table.Column<string>(type: "text", nullable: false),
                    Traits_AvatarPath = table.Column<string>(type: "text", nullable: false),
                    Traits_Description = table.Column<string>(type: "text", nullable: false),
                    Hobbies = table.Column<string>(type: "text", nullable: false),
                    Traits_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Traits_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

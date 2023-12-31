﻿#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyBuddy.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixMatchRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchRequests",
                table: "MatchRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchRequests",
                table: "MatchRequests",
                columns: new[] { "RequesterId", "RequestedId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchRequests",
                table: "MatchRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchRequests",
                table: "MatchRequests",
                column: "RequesterId");
        }
    }
}

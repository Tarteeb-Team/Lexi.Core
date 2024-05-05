using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lexi.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class ba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImprovedSpeech",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImprovedSpeech",
                table: "Users");
        }
    }
}

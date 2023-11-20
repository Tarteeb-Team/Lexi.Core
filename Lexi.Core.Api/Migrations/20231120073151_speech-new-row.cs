using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lexi.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class speechnewrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SpeechId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SpeechId",
                table: "Feedbacks",
                column: "SpeechId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SpeechId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SpeechId",
                table: "Feedbacks",
                column: "SpeechId");
        }
    }
}

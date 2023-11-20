using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lexi.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateAllTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Accuracy = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fluency = table.Column<decimal>(type: "TEXT", nullable: false),
                    Prosody = table.Column<decimal>(type: "TEXT", nullable: false),
                    Complenteness = table.Column<decimal>(type: "TEXT", nullable: false),
                    Pronunciation = table.Column<decimal>(type: "TEXT", nullable: false),
                    SpeechId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Speeches_SpeechId",
                        column: x => x.SpeechId,
                        principalTable: "Speeches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SpeechId",
                table: "Feedbacks",
                column: "SpeechId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");
        }
    }
}

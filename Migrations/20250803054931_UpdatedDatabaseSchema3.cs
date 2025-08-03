using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabaseSchema3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuideNotes_DepartureDates",
                table: "GuideNotes");

            migrationBuilder.DropIndex(
                name: "IX_GuideNotes_DepartureDateId",
                table: "GuideNotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GuideNotes_DepartureDateId",
                table: "GuideNotes",
                column: "DepartureDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuideNotes_DepartureDates",
                table: "GuideNotes",
                column: "DepartureDateId",
                principalTable: "DepartureDates",
                principalColumn: "Id");
        }
    }
}

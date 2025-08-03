using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabaseSchema2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourGuideAssignments_Tours",
                table: "TourGuideAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TourGuideAssignments_TourId",
                table: "TourGuideAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAssignments_TourId",
                table: "TourGuideAssignments",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_TourGuideAssignments_Tours",
                table: "TourGuideAssignments",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId");
        }
    }
}

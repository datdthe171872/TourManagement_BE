using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ResetPasswordTokens",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRatings_TourGuideId",
                table: "GuideRatings",
                column: "TourGuideId");

            migrationBuilder.AddForeignKey(
                name: "FK__GuideRati__TourG__4A8310C6",
                table: "GuideRatings",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "TourGuideId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__GuideRati__TourG__4A8310C6",
                table: "GuideRatings");

            migrationBuilder.DropIndex(
                name: "IX_GuideRatings_TourGuideId",
                table: "GuideRatings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ResetPasswordTokens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");
        }
    }
}

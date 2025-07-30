using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TourType",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "DurationInYears",
                table: "ServicePackages");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SelectedDepartureDate",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Tours",
                newName: "PriceOfInfants");

            migrationBuilder.AddColumn<int>(
                name: "MinSlots",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOfAdults",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOfChildren",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "ResetPasswordTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentCode",
                table: "PurchaseTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartureDateId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordTokens_UserId1",
                table: "ResetPasswordTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DepartureDateId",
                table: "Bookings",
                column: "DepartureDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DepartureDates_DepartureDateId",
                table: "Bookings",
                column: "DepartureDateId",
                principalTable: "DepartureDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordTokens_Users_UserId1",
                table: "ResetPasswordTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DepartureDates_DepartureDateId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordTokens_Users_UserId1",
                table: "ResetPasswordTokens");

            migrationBuilder.DropIndex(
                name: "IX_ResetPasswordTokens_UserId1",
                table: "ResetPasswordTokens");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DepartureDateId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MinSlots",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PriceOfAdults",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PriceOfChildren",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ResetPasswordTokens");

            migrationBuilder.DropColumn(
                name: "ContentCode",
                table: "PurchaseTransactions");

            migrationBuilder.DropColumn(
                name: "DepartureDateId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PriceOfInfants",
                table: "Tours",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "TourType",
                table: "Tours",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Shared");

            migrationBuilder.AddColumn<int>(
                name: "DurationInYears",
                table: "ServicePackages",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "SelectedDepartureDate",
                table: "Bookings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

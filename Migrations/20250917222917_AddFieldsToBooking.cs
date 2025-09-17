using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentImg",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentImg",
                table: "Bookings");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterPull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DepartureDates_DepartureDateId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK__ResetPass__UserI__12345678",
                table: "ResetPasswordTokens");

            migrationBuilder.DropTable(
                name: "BookingExtraCharges");

            migrationBuilder.DropTable(
                name: "ExtraCharges");

            migrationBuilder.DropIndex(
                name: "UQ__TourOper__1788CC4D657A0885",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "UQ__TourGuid__1788CC4D814900F4",
                table: "TourGuides");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ResetPas__3214EC07ABCD1234",
                table: "ResetPasswordTokens");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "TourAcceptanceReports");

            migrationBuilder.DropColumn(
                name: "MaxTours",
                table: "ServicePackages");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ResetPasswordTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "ResetPasswordTokens",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ResetPasswordTokens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "ContentCode",
                table: "PurchaseTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraCost",
                table: "GuideNotes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "GuideNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordTokens",
                table: "ResetPasswordTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ServicePackageFeatures",
                columns: table => new
                {
                    FeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    FeatureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeatureValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceP__82230BC98B4657F8", x => x.FeatureId);
                    table.ForeignKey(
                        name: "FK__ServicePa__Packa__5CD6CB2B",
                        column: x => x.PackageId,
                        principalTable: "ServicePackages",
                        principalColumn: "PackageId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__TourOper__1788CC4D657A0885",
                table: "TourOperators",
                column: "UserId",
                unique: true,
                filter: "([UserId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UQ__TourGuid__1788CC4D814900F4",
                table: "TourGuides",
                column: "UserId",
                unique: true,
                filter: "([UserId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_GuideNotes_ReportId",
                table: "GuideNotes",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackageFeatures_PackageId",
                table: "ServicePackageFeatures",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DepartureDates",
                table: "Bookings",
                column: "DepartureDateId",
                principalTable: "DepartureDates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuideNotes_TourAcceptanceReports",
                table: "GuideNotes",
                column: "ReportId",
                principalTable: "TourAcceptanceReports",
                principalColumn: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordTokens_Users_UserId",
                table: "ResetPasswordTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DepartureDates",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GuideNotes_TourAcceptanceReports",
                table: "GuideNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordTokens_Users_UserId",
                table: "ResetPasswordTokens");

            migrationBuilder.DropTable(
                name: "ServicePackageFeatures");

            migrationBuilder.DropIndex(
                name: "UQ__TourOper__1788CC4D657A0885",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "UQ__TourGuid__1788CC4D814900F4",
                table: "TourGuides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordTokens",
                table: "ResetPasswordTokens");

            migrationBuilder.DropIndex(
                name: "IX_GuideNotes_ReportId",
                table: "GuideNotes");

            migrationBuilder.DropColumn(
                name: "ExtraCost",
                table: "GuideNotes");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "GuideNotes");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "TourAcceptanceReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTours",
                table: "ServicePackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ResetPasswordTokens",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "ResetPasswordTokens",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ResetPasswordTokens",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ContentCode",
                table: "PurchaseTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ResetPas__3214EC07ABCD1234",
                table: "ResetPasswordTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExtraCharges",
                columns: table => new
                {
                    ExtraChargeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExtraCha__23A84331F76C775E", x => x.ExtraChargeId);
                });

            migrationBuilder.CreateTable(
                name: "BookingExtraCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ExtraChargeId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingE__3214EC072D708871", x => x.Id);
                    table.ForeignKey(
                        name: "FK__BookingEx__Booki__40058253",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__BookingEx__Extra__40F9A68C",
                        column: x => x.ExtraChargeId,
                        principalTable: "ExtraCharges",
                        principalColumn: "ExtraChargeId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__TourOper__1788CC4D657A0885",
                table: "TourOperators",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__TourGuid__1788CC4D814900F4",
                table: "TourGuides",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingExtraCharges_BookingId",
                table: "BookingExtraCharges",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingExtraCharges_ExtraChargeId",
                table: "BookingExtraCharges",
                column: "ExtraChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DepartureDates_DepartureDateId",
                table: "Bookings",
                column: "DepartureDateId",
                principalTable: "DepartureDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ResetPass__UserI__12345678",
                table: "ResetPasswordTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

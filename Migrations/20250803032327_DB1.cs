using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class DB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Language__B93855AB17FB757D", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentT__BA430B35A0EA8336", x => x.PaymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE1A793714B3", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ServicePackages",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    MaxTour = table.Column<int>(type: "int", nullable: false),
                    MaxImage = table.Column<int>(type: "int", nullable: false),
                    MaxVideo = table.Column<bool>(type: "bit", nullable: false),
                    TourGuideFunction = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceP__322035CC2CFB51A1", x => x.PackageId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4C6EA58613", x => x.UserId);
                    table.ForeignKey(
                        name: "FK__Users__RoleId__6442E2C9",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                });

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

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedEntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20B2F72112345678", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__6555E2C9",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResetPasswordTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourOperators",
                columns: table => new
                {
                    TourOperatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyLogo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LicenseIssuedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstablishedYear = table.Column<int>(type: "int", nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Instagram = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WorkingHours = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourOper__776E46D99CB4F975", x => x.TourOperatorId);
                    table.ForeignKey(
                        name: "FK__TourOpera__UserI__607251E5",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourOperatorId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    ContentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__55433A6B7EA420F9", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK__PurchaseT__Packa__51300E55",
                        column: x => x.PackageId,
                        principalTable: "ServicePackages",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK__PurchaseT__TourO__5224328E",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "TourGuides",
                columns: table => new
                {
                    TourGuideId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    TourOperatorId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourGuid__2F0E035344C0797A", x => x.TourGuideId);
                    table.ForeignKey(
                        name: "FK__TourGuide__TourO__5BAD9CC8",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                    table.ForeignKey(
                        name: "FK__TourGuide__UserI__5CA1C101",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TourOperatorMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourOperatorId = table.Column<int>(type: "int", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourOper__3214EC076E5C56CE", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourOpera__TourO__5F7E2DAC",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceOfAdults = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceOfChildren = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceOfInfants = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInDays = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartPoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Transportation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TourOperatorId = table.Column<int>(type: "int", nullable: false),
                    MaxSlots = table.Column<int>(type: "int", nullable: false),
                    MinSlots = table.Column<int>(type: "int", nullable: false),
                    SlotsBooked = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TourStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Active"),
                    TourAvartar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tours__604CEA3014B7154A", x => x.TourId);
                    table.ForeignKey(
                        name: "FK__Tours__TourOpera__634EBE90",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "PurchasedServicePackages",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourOperatorId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    NumOfToursUsed = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__6B0A6BBE22291CBE", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK__Purchased__Packa__4E53A1AA",
                        column: x => x.PackageId,
                        principalTable: "ServicePackages",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK__Purchased__TourO__4F47C5E3",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                    table.ForeignKey(
                        name: "FK__Purchased__Trans__503BEA1C",
                        column: x => x.TransactionId,
                        principalTable: "PurchaseTransactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateTable(
                name: "GuideLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuideId = table.Column<int>(type: "int", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideLan__3214EC0754BC5DEE", x => x.Id);
                    table.ForeignKey(
                        name: "FK__GuideLang__Guide__44CA3770",
                        column: x => x.GuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                    table.ForeignKey(
                        name: "FK__GuideLang__Langu__45BE5BA9",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId");
                });

            migrationBuilder.CreateTable(
                name: "TourGuideAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    DepartureDateId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    NoteId = table.Column<int>(type: "int", nullable: true),
                    IsLeadGuide = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourGuid__3214EC073C3A4EB5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourGuide__TourG__5AB9788F",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                });

            migrationBuilder.CreateTable(
                name: "DepartureDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsCancelDate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departur__3214EC07D728106D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Departure__TourI__43D61337",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "SavedTours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SavedTou__3214EC0795DB6045", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SavedTour__TourI__531856C7",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__SavedTour__UserI__540C7B00",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TourCancellations",
                columns: table => new
                {
                    CancellationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    DepartureDateId = table.Column<int>(type: "int", nullable: false),
                    CancelledBy = table.Column<int>(type: "int", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourCanc__6A2D9A3A1E33ECEB", x => x.CancellationId);
                    table.ForeignKey(
                        name: "FK__TourCance__Cance__56E8E7AB",
                        column: x => x.CancelledBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK__TourCance__TourI__57DD0BE4",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourExperiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourExpe__3214EC0771B42547", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourExper__TourI__58D1301D",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourItineraries",
                columns: table => new
                {
                    ItineraryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourItin__361216C6A8FF1462", x => x.ItineraryId);
                    table.ForeignKey(
                        name: "FK__TourItine__TourI__5D95E53A",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourMedi__3214EC07762CE9DE", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourMedia__TourI__5E8A0973",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourRatings",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourRati__FCCDF87C99F24843", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK__TourRatin__TourI__6166761E",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__TourRatin__UserI__625A9A57",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "GuideRatings",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideRat__FCCDF87C1044E830", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK__GuideRati__Assig__489AC854",
                        column: x => x.AssignmentId,
                        principalTable: "TourGuideAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__GuideRati__UserI__498EEC8D",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    DepartureDateId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    NumberOfAdults = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NumberOfInfants = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NoteForTour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Contract = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bookings__73951AED69F9C654", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_DepartureDates",
                        column: x => x.DepartureDateId,
                        principalTable: "DepartureDates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Bookings__TourId__41EDCAC5",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__Bookings__UserId__42E1EEFE",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ItineraryMedia",
                columns: table => new
                {
                    MediaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItineraryId = table.Column<int>(type: "int", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Itinerar__B2C2B5CFEA6D2966", x => x.MediaId);
                    table.ForeignKey(
                        name: "FK__Itinerary__Itine__4A8310C6",
                        column: x => x.ItineraryId,
                        principalTable: "TourItineraries",
                        principalColumn: "ItineraryId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__9B556A389E758151", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK__Payments__Bookin__4B7734FF",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__Payments__Paymen__4C6B5938",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "PaymentTypeId");
                    table.ForeignKey(
                        name: "FK__Payments__UserId__4D5F7D71",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TourAcceptanceReports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TotalExtraCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourAcce__D5BD480579C896E8", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK__TourAccep__Booki__55009F39",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__TourAccep__TourG__55F4C372",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                });

            migrationBuilder.CreateTable(
                name: "GuideNotes",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DepartureDateId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideNot__EACE355F7D9F9B80", x => x.NoteId);
                    table.ForeignKey(
                        name: "FK_GuideNotes_TourAcceptanceReports",
                        column: x => x.ReportId,
                        principalTable: "TourAcceptanceReports",
                        principalColumn: "ReportId");
                    table.ForeignKey(
                        name: "FK__GuideNote__Assig__47A6A41B",
                        column: x => x.AssignmentId,
                        principalTable: "TourGuideAssignments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuideNoteMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteId = table.Column<int>(type: "int", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideNot__3214EC078F6A2553", x => x.Id);
                    table.ForeignKey(
                        name: "FK__GuideNote__NoteI__46B27FE2",
                        column: x => x.NoteId,
                        principalTable: "GuideNotes",
                        principalColumn: "NoteId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DepartureDateId",
                table: "Bookings",
                column: "DepartureDateId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartureDates_TourId",
                table: "DepartureDates",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideLanguages_GuideId",
                table: "GuideLanguages",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideLanguages_LanguageId",
                table: "GuideLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideNoteMedia_NoteId",
                table: "GuideNoteMedia",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideNotes_AssignmentId",
                table: "GuideNotes",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideNotes_ReportId",
                table: "GuideNotes",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRatings_AssignmentId",
                table: "GuideRatings",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRatings_UserId",
                table: "GuideRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryMedia_ItineraryId",
                table: "ItineraryMedia",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTypeId",
                table: "Payments",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedServicePackages_PackageId",
                table: "PurchasedServicePackages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedServicePackages_TourOperatorId",
                table: "PurchasedServicePackages",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedServicePackages_TransactionId",
                table: "PurchasedServicePackages",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_PackageId",
                table: "PurchaseTransactions",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_TourOperatorId",
                table: "PurchaseTransactions",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordTokens_UserId",
                table: "ResetPasswordTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__8A2B616060CD7784",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedTours_TourId",
                table: "SavedTours",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedTours_UserId",
                table: "SavedTours",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackageFeatures_PackageId",
                table: "ServicePackageFeatures",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TourAcceptanceReports_BookingId",
                table: "TourAcceptanceReports",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_TourAcceptanceReports_TourGuideId",
                table: "TourAcceptanceReports",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourCancellations_CancelledBy",
                table: "TourCancellations",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_TourCancellations_TourId",
                table: "TourCancellations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExperiences_TourId",
                table: "TourExperiences",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAssignments_TourGuideId",
                table: "TourGuideAssignments",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuides_TourOperatorId",
                table: "TourGuides",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "UQ__TourGuid__1788CC4D814900F4",
                table: "TourGuides",
                column: "UserId",
                unique: true,
                filter: "([UserId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_TourItineraries_TourId",
                table: "TourItineraries",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourMedia_TourId",
                table: "TourMedia",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourOperatorMedia_TourOperatorId",
                table: "TourOperatorMedia",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "UQ__TourOper__1788CC4D657A0885",
                table: "TourOperators",
                column: "UserId",
                unique: true,
                filter: "([UserId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_TourRatings_TourId",
                table: "TourRatings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourRatings_UserId",
                table: "TourRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_TourOperatorId",
                table: "Tours",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D10534A733E8F8",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuideLanguages");

            migrationBuilder.DropTable(
                name: "GuideNoteMedia");

            migrationBuilder.DropTable(
                name: "GuideRatings");

            migrationBuilder.DropTable(
                name: "ItineraryMedia");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PurchasedServicePackages");

            migrationBuilder.DropTable(
                name: "ResetPasswordTokens");

            migrationBuilder.DropTable(
                name: "SavedTours");

            migrationBuilder.DropTable(
                name: "ServicePackageFeatures");

            migrationBuilder.DropTable(
                name: "TourCancellations");

            migrationBuilder.DropTable(
                name: "TourExperiences");

            migrationBuilder.DropTable(
                name: "TourMedia");

            migrationBuilder.DropTable(
                name: "TourOperatorMedia");

            migrationBuilder.DropTable(
                name: "TourRatings");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "GuideNotes");

            migrationBuilder.DropTable(
                name: "TourItineraries");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "PurchaseTransactions");

            migrationBuilder.DropTable(
                name: "TourAcceptanceReports");

            migrationBuilder.DropTable(
                name: "TourGuideAssignments");

            migrationBuilder.DropTable(
                name: "ServicePackages");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "TourGuides");

            migrationBuilder.DropTable(
                name: "DepartureDates");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "TourOperators");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement_BE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtraCharges",
                columns: table => new
                {
                    ExtraChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExtraCha__23A8433163E6102E", x => x.ExtraChargeId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Language__B93855ABC3CC294B", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    PaymentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentT__BA430B353FC67E2A", x => x.PaymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE1ADAE76122", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ServicePackages",
                columns: table => new
                {
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    DurationInYears = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    MaxTours = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceP__322035CC8E4950CF", x => x.PackageId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4CE4A6A2AC", x => x.UserId);
                    table.ForeignKey(
                        name: "FK__Users__RoleId__4D94879B",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "TourOperators",
                columns: table => new
                {
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourOper__776E46D942697525", x => x.TourOperatorId);
                    table.ForeignKey(
                        name: "FK__TourOpera__UserI__52593CB8",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__55433A6BA7168FAB", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK__PurchaseT__Packa__693CA210",
                        column: x => x.PackageId,
                        principalTable: "ServicePackages",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK__PurchaseT__TourO__68487DD7",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "TourGuides",
                columns: table => new
                {
                    TourGuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourGuid__2F0E03537ACD676D", x => x.TourGuideId);
                    table.ForeignKey(
                        name: "FK__TourGuide__TourO__5812160E",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                    table.ForeignKey(
                        name: "FK__TourGuide__UserI__571DF1D5",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInDays = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartPoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Transportation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxSlots = table.Column<int>(type: "int", nullable: false),
                    SlotsBooked = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TourStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Active"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tours__604CEA3081C3056D", x => x.TourId);
                    table.ForeignKey(
                        name: "FK__Tours__TourOpera__778AC167",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "PurchasedServicePackages",
                columns: table => new
                {
                    PurchaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NumOfToursUsed = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__6B0A6BBE338EF296", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK__Purchased__Packa__6FE99F9F",
                        column: x => x.PackageId,
                        principalTable: "ServicePackages",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK__Purchased__TourO__6EF57B66",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                    table.ForeignKey(
                        name: "FK__Purchased__Trans__70DDC3D8",
                        column: x => x.TransactionId,
                        principalTable: "PurchaseTransactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateTable(
                name: "GuideLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideLan__3214EC073D7D8576", x => x.Id);
                    table.ForeignKey(
                        name: "FK__GuideLang__Guide__5DCAEF64",
                        column: x => x.GuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                    table.ForeignKey(
                        name: "FK__GuideLang__Langu__5EBF139D",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId");
                });

            migrationBuilder.CreateTable(
                name: "GuideRatings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourGuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideRat__FCCDF87C23B2F91C", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK__GuideRati__TourG__2BFE89A6",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                    table.ForeignKey(
                        name: "FK__GuideRati__UserI__2CF2ADDF",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    NumberOfAdults = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NumberOfInfants = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NoteForTour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    BookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bookings__73951AED7E713091", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK__Bookings__TourId__09A971A2",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__Bookings__UserId__08B54D69",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "DepartureDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartureDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departur__3214EC07DA56838E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Departure__TourI__00200768",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "SavedTours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SavedTou__3214EC0763387A86", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SavedTour__TourI__2180FB33",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__SavedTour__UserI__208CD6FA",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TourCancellations",
                columns: table => new
                {
                    CancellationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelledBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourCanc__6A2D9A3A541BFFB0", x => x.CancellationId);
                    table.ForeignKey(
                        name: "FK__TourCance__Cance__3864608B",
                        column: x => x.CancelledBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK__TourCance__TourI__37703C52",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourExpe__3214EC07E24EFC16", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourExper__TourI__05D8E0BE",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourGuideAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourGuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLeadGuide = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourGuid__3214EC071CAE5430", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourGuide__TourG__7D439ABD",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                    table.ForeignKey(
                        name: "FK__TourGuide__TourI__7C4F7684",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourImag__3214EC079DC64CF5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourImage__TourI__02FC7413",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateTable(
                name: "TourRatings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourRati__FCCDF87C7EC51D83", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK__TourRatin__TourI__2645B050",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId");
                    table.ForeignKey(
                        name: "FK__TourRatin__UserI__2739D489",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BookingExtraCharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingE__3214EC0770CA08A8", x => x.Id);
                    table.ForeignKey(
                        name: "FK__BookingEx__Booki__1BC821DD",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__BookingEx__Extra__1CBC4616",
                        column: x => x.ExtraChargeId,
                        principalTable: "ExtraCharges",
                        principalColumn: "ExtraChargeId");
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourOperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractFileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsSignedByCustomer = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    SignedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByTourOperator = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contract__C90D3469906515E3", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK__Contracts__Booki__32AB8735",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__Contracts__TourO__339FAB6E",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "TourOperatorId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PaymentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__9B556A38336B94E0", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK__Payments__Bookin__40058253",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__Payments__Paymen__41EDCAC5",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "PaymentTypeId");
                    table.ForeignKey(
                        name: "FK__Payments__UserId__40F9A68C",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TourCompletionReports",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourGuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalExtraCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TourComp__D5BD4805EDF8A396", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK__TourCompl__Booki__46B27FE2",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__TourCompl__TourG__47A6A41B",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "TourGuideId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingExtraCharges_BookingId",
                table: "BookingExtraCharges",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingExtraCharges_ExtraChargeId",
                table: "BookingExtraCharges",
                column: "ExtraChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_BookingId",
                table: "Contracts",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TourOperatorId",
                table: "Contracts",
                column: "TourOperatorId");

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
                name: "IX_GuideRatings_TourGuideId",
                table: "GuideRatings",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRatings_UserId",
                table: "GuideRatings",
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
                name: "UQ__Roles__8A2B6160AA8BB858",
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
                name: "IX_TourCancellations_CancelledBy",
                table: "TourCancellations",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_TourCancellations_TourId",
                table: "TourCancellations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourCompletionReports_BookingId",
                table: "TourCompletionReports",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_TourCompletionReports_TourGuideId",
                table: "TourCompletionReports",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExperiences_TourId",
                table: "TourExperiences",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAssignments_TourGuideId",
                table: "TourGuideAssignments",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAssignments_TourId",
                table: "TourGuideAssignments",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuides_TourOperatorId",
                table: "TourGuides",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "UQ__TourGuid__1788CC4D40106782",
                table: "TourGuides",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourImages_TourId",
                table: "TourImages",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "UQ__TourOper__1788CC4DD9E06591",
                table: "TourOperators",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

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
                name: "UQ__Users__A9D105345EB09E15",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingExtraCharges");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "DepartureDates");

            migrationBuilder.DropTable(
                name: "GuideLanguages");

            migrationBuilder.DropTable(
                name: "GuideRatings");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PurchasedServicePackages");

            migrationBuilder.DropTable(
                name: "SavedTours");

            migrationBuilder.DropTable(
                name: "TourCancellations");

            migrationBuilder.DropTable(
                name: "TourCompletionReports");

            migrationBuilder.DropTable(
                name: "TourExperiences");

            migrationBuilder.DropTable(
                name: "TourGuideAssignments");

            migrationBuilder.DropTable(
                name: "TourImages");

            migrationBuilder.DropTable(
                name: "TourRatings");

            migrationBuilder.DropTable(
                name: "ExtraCharges");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "PurchaseTransactions");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "TourGuides");

            migrationBuilder.DropTable(
                name: "ServicePackages");

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

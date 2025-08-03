using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.Context;

public partial class MyDBContext : DbContext
{
    public MyDBContext()
    {
    }

    public MyDBContext(DbContextOptions<MyDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<DepartureDate> DepartureDates { get; set; }

    public virtual DbSet<GuideLanguage> GuideLanguages { get; set; }

    public virtual DbSet<GuideNote> GuideNotes { get; set; }

    public virtual DbSet<GuideNoteMedia> GuideNoteMedia { get; set; }

    public virtual DbSet<GuideRating> GuideRatings { get; set; }

    public virtual DbSet<ItineraryMedia> ItineraryMedia { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }

    public virtual DbSet<PurchasedServicePackage> PurchasedServicePackages { get; set; }

    public virtual DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedTour> SavedTours { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<ServicePackageFeature> ServicePackageFeatures { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourAcceptanceReport> TourAcceptanceReports { get; set; }

    public virtual DbSet<TourExperience> TourExperiences { get; set; }

    public virtual DbSet<TourGuide> TourGuides { get; set; }

    public virtual DbSet<TourGuideAssignment> TourGuideAssignments { get; set; }

    public virtual DbSet<TourItinerary> TourItineraries { get; set; }

    public virtual DbSet<TourMedia> TourMedia { get; set; }

    public virtual DbSet<TourOperator> TourOperators { get; set; }

    public virtual DbSet<TourOperatorMedia> TourOperatorMedia { get; set; }

    public virtual DbSet<TourRating> TourRatings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MSI\\SQLEXPRESS;Initial Catalog=SEP490_G35; Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED69F9C654");

            entity.HasIndex(e => e.DepartureDateId, "IX_Bookings_DepartureDateId");

            entity.HasIndex(e => e.TourId, "IX_Bookings_TourId");

            entity.HasIndex(e => e.UserId, "IX_Bookings_UserId");

            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Contract).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NumberOfAdults).HasDefaultValue(0);
            entity.Property(e => e.NumberOfChildren).HasDefaultValue(0);
            entity.Property(e => e.NumberOfInfants).HasDefaultValue(0);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.DepartureDate).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DepartureDateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_DepartureDates");

            entity.HasOne(d => d.Tour).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__TourId__41EDCAC5");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__42E1EEFE");
        });

        modelBuilder.Entity<DepartureDate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departur__3214EC07D728106D");

            entity.HasIndex(e => e.TourId, "IX_DepartureDates_TourId");

            entity.Property(e => e.DepartureDate1)
                .HasColumnType("datetime")
                .HasColumnName("DepartureDate");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Tour).WithMany(p => p.DepartureDates)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departure__TourI__43D61337");
        });

        modelBuilder.Entity<GuideLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GuideLan__3214EC0754BC5DEE");

            entity.HasIndex(e => e.GuideId, "IX_GuideLanguages_GuideId");

            entity.HasIndex(e => e.LanguageId, "IX_GuideLanguages_LanguageId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Guide).WithMany(p => p.GuideLanguages)
                .HasForeignKey(d => d.GuideId)
                .HasConstraintName("FK__GuideLang__Guide__44CA3770");

            entity.HasOne(d => d.Language).WithMany(p => p.GuideLanguages)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK__GuideLang__Langu__45BE5BA9");
        });

        modelBuilder.Entity<GuideNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__GuideNot__EACE355F7D9F9B80");

            entity.HasIndex(e => e.AssignmentId, "IX_GuideNotes_AssignmentId");

            entity.HasIndex(e => e.BookingId, "IX_GuideNotes_BookingId");

            entity.HasIndex(e => e.ReportId, "IX_GuideNotes_ReportId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExtraCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Assignment).WithMany(p => p.GuideNotes)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideNote__Assig__47A6A41B");

            entity.HasOne(d => d.Booking).WithMany(p => p.GuideNotes)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideNotes_Bookings");

            entity.HasOne(d => d.Report).WithMany(p => p.GuideNotes)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideNotes_TourAcceptanceReports");
        });

        modelBuilder.Entity<GuideNoteMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GuideNot__3214EC078F6A2553");

            entity.HasIndex(e => e.NoteId, "IX_GuideNoteMedia_NoteId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Note).WithMany(p => p.GuideNoteMedia)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideNote__NoteI__46B27FE2");
        });

        modelBuilder.Entity<GuideRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__GuideRat__FCCDF87C1044E830");

            entity.HasIndex(e => e.AssignmentId, "IX_GuideRatings_AssignmentId");

            entity.HasIndex(e => e.TourGuideId, "IX_GuideRatings_TourGuideId");

            entity.HasIndex(e => e.UserId, "IX_GuideRatings_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);

            entity.HasOne(d => d.Assignment).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideRati__Assig__489AC854");

            entity.HasOne(d => d.TourGuide).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideRati__TourG__4A8310C6");

            entity.HasOne(d => d.User).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideRati__UserI__498EEC8D");
        });

        modelBuilder.Entity<ItineraryMedia>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__Itinerar__B2C2B5CFEA6D2966");

            entity.HasIndex(e => e.ItineraryId, "IX_ItineraryMedia_ItineraryId");

            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaType).HasMaxLength(50);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.ItineraryMedia)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Itine__4A8310C6");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B93855AB17FB757D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LanguageName).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20B2F72112345678");

            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.RelatedEntityId).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__6555E2C9");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A389E758151");

            entity.HasIndex(e => e.BookingId, "IX_Payments_BookingId");

            entity.HasIndex(e => e.PaymentTypeId, "IX_Payments_PaymentTypeId");

            entity.HasIndex(e => e.UserId, "IX_Payments_UserId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AmountPaid)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentReference).HasMaxLength(255);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Bookin__4B7734FF");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__4C6B5938");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserId__4D5F7D71");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentTypeId).HasName("PK__PaymentT__BA430B35A0EA8336");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<PurchaseTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Purchase__55433A6B7EA420F9");

            entity.HasIndex(e => e.PackageId, "IX_PurchaseTransactions_PackageId");

            entity.HasIndex(e => e.TourOperatorId, "IX_PurchaseTransactions_TourOperatorId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ContentCode).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Package).WithMany(p => p.PurchaseTransactions)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseT__Packa__51300E55");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.PurchaseTransactions)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseT__TourO__5224328E");
        });

        modelBuilder.Entity<PurchasedServicePackage>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBE22291CBE");

            entity.HasIndex(e => e.PackageId, "IX_PurchasedServicePackages_PackageId");

            entity.HasIndex(e => e.TourOperatorId, "IX_PurchasedServicePackages_TourOperatorId");

            entity.HasIndex(e => e.TransactionId, "IX_PurchasedServicePackages_TransactionId");

            entity.Property(e => e.ActivationDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NumOfToursUsed).HasDefaultValue(0);

            entity.HasOne(d => d.Package).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__Packa__4E53A1AA");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__TourO__4F47C5E3");

            entity.HasOne(d => d.Transaction).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__Trans__503BEA1C");
        });

        modelBuilder.Entity<ResetPasswordToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ResetPasswordTokens");

            entity.HasIndex(e => e.UserId, "IX_ResetPasswordTokens_UserId");

            entity.Property(e => e.ExpiryDate).HasColumnType("datetime2(7)");

            entity.HasOne(d => d.User).WithMany(p => p.ResetPasswordTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ResetPasswordTokens_Users_UserId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A793714B3");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616060CD7784").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SavedTour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SavedTou__3214EC0795DB6045");

            entity.HasIndex(e => e.TourId, "IX_SavedTours_TourId");

            entity.HasIndex(e => e.UserId, "IX_SavedTours_UserId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SavedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tour).WithMany(p => p.SavedTours)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SavedTour__TourI__531856C7");

            entity.HasOne(d => d.User).WithMany(p => p.SavedTours)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SavedTour__UserI__540C7B00");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__ServiceP__322035CC2CFB51A1");

            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ServicePackageFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__ServiceP__82230BC98B4657F8");

            entity.HasIndex(e => e.PackageId, "IX_ServicePackageFeatures_PackageId");

            entity.Property(e => e.FeatureName).HasMaxLength(100);

            entity.HasOne(d => d.Package).WithMany(p => p.ServicePackageFeatures)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicePa__Packa__5CD6CB2B");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("PK__Tours__604CEA3014B7154A");

            entity.HasIndex(e => e.TourOperatorId, "IX_Tours_TourOperatorId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DurationInDays).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PriceOfAdults).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceOfChildren).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceOfInfants).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SlotsBooked).HasDefaultValue(0);
            entity.Property(e => e.StartPoint).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.TourAvartar).HasMaxLength(255);
            entity.Property(e => e.TourStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.Transportation).HasMaxLength(255);

            entity.HasOne(d => d.TourOperator).WithMany(p => p.Tours)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tours__TourOpera__634EBE90");
        });

        modelBuilder.Entity<TourAcceptanceReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__TourAcce__D5BD480579C896E8");

            entity.HasIndex(e => e.BookingId, "IX_TourAcceptanceReports_BookingId");

            entity.HasIndex(e => e.TourGuideId, "IX_TourAcceptanceReports_TourGuideId");

            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ReportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalExtraCost)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.TourAcceptanceReports)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourAccep__Booki__55009F39");

            entity.HasOne(d => d.TourGuide).WithMany(p => p.TourAcceptanceReports)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourAccep__TourG__55F4C372");
        });

        modelBuilder.Entity<TourExperience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourExpe__3214EC0771B42547");

            entity.HasIndex(e => e.TourId, "IX_TourExperiences_TourId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourExperiences)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourExper__TourI__58D1301D");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity.HasKey(e => e.TourGuideId).HasName("PK__TourGuid__2F0E035344C0797A");

            entity.HasIndex(e => e.TourOperatorId, "IX_TourGuides_TourOperatorId");

            entity.HasIndex(e => e.UserId, "UQ__TourGuid__1788CC4D814900F4")
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.TourOperator).WithMany(p => p.TourGuides)
                .HasForeignKey(d => d.TourOperatorId)
                .HasConstraintName("FK__TourGuide__TourO__5BAD9CC8");

            entity.HasOne(d => d.User).WithOne(p => p.TourGuide)
                .HasForeignKey<TourGuide>(d => d.UserId)
                .HasConstraintName("FK__TourGuide__UserI__5CA1C101");
        });

        modelBuilder.Entity<TourGuideAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourGuid__3214EC073C3A4EB5");

            entity.HasIndex(e => e.DepartureDateId, "IX_TourGuideAssignments_DepartureDateId");

            entity.HasIndex(e => e.TourGuideId, "IX_TourGuideAssignments_TourGuideId");

            entity.Property(e => e.AssignedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsLeadGuide).HasDefaultValue(false);

            entity.HasOne(d => d.DepartureDate).WithMany(p => p.TourGuideAssignments)
                .HasForeignKey(d => d.DepartureDateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourGuideAssignments_DepartureDates");

            entity.HasOne(d => d.TourGuide).WithMany(p => p.TourGuideAssignments)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourGuide__TourG__5AB9788F");
        });

        modelBuilder.Entity<TourItinerary>(entity =>
        {
            entity.HasKey(e => e.ItineraryId).HasName("PK__TourItin__361216C6A8FF1462");

            entity.HasIndex(e => e.TourId, "IX_TourItineraries_TourId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourItineraries)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourItine__TourI__5D95E53A");
        });

        modelBuilder.Entity<TourMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourMedi__3214EC07762CE9DE");

            entity.HasIndex(e => e.TourId, "IX_TourMedia_TourId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaType).HasMaxLength(255);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourMedia)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourMedia__TourI__5E8A0973");
        });

        modelBuilder.Entity<TourOperator>(entity =>
        {
            entity.HasKey(e => e.TourOperatorId).HasName("PK__TourOper__776E46D99CB4F975");

            entity.HasIndex(e => e.UserId, "UQ__TourOper__1788CC4D657A0885")
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CompanyLogo).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.Facebook).HasMaxLength(255);
            entity.Property(e => e.Hotline).HasMaxLength(100);
            entity.Property(e => e.Instagram).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LicenseNumber).HasMaxLength(100);
            entity.Property(e => e.TaxCode).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(255);
            entity.Property(e => e.WorkingHours).HasMaxLength(255);

            entity.HasOne(d => d.User).WithOne(p => p.TourOperator)
                .HasForeignKey<TourOperator>(d => d.UserId)
                .HasConstraintName("FK__TourOpera__UserI__607251E5");
        });

        modelBuilder.Entity<TourOperatorMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourOper__3214EC076E5C56CE");

            entity.HasIndex(e => e.TourOperatorId, "IX_TourOperatorMedia_TourOperatorId");

            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.TourOperatorMedia)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourOpera__TourO__5F7E2DAC");
        });

        modelBuilder.Entity<TourRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__TourRati__FCCDF87C99F24843");

            entity.HasIndex(e => e.TourId, "IX_TourRatings_TourId");

            entity.HasIndex(e => e.UserId, "IX_TourRatings_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourRatings)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourRatin__TourI__6166761E");

            entity.HasOne(d => d.User).WithMany(p => p.TourRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourRatin__UserI__625A9A57");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6EA58613");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A733E8F8").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__6442E2C9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

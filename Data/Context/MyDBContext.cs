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

    public virtual DbSet<BookingExtraCharge> BookingExtraCharges { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<DepartureDate> DepartureDates { get; set; }

    public virtual DbSet<ExtraCharge> ExtraCharges { get; set; }

    public virtual DbSet<GuideLanguage> GuideLanguages { get; set; }

    public virtual DbSet<GuideRating> GuideRatings { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }

    public virtual DbSet<PurchasedServicePackage> PurchasedServicePackages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedTour> SavedTours { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourCancellation> TourCancellations { get; set; }

    public virtual DbSet<TourCompletionReport> TourCompletionReports { get; set; }

    public virtual DbSet<TourExperience> TourExperiences { get; set; }

    public virtual DbSet<TourGuide> TourGuides { get; set; }

    public virtual DbSet<TourGuideAssignment> TourGuideAssignments { get; set; }

    public virtual DbSet<TourImage> TourImages { get; set; }

    public virtual DbSet<TourOperator> TourOperators { get; set; }

    public virtual DbSet<TourRating> TourRatings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // RẤT QUAN TRỌNG!

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED7E713091");

            entity.Property(e => e.BookingId).ValueGeneratedNever();
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.DepositAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumberOfAdults).HasDefaultValue(1);
            entity.Property(e => e.NumberOfChildren).HasDefaultValue(0);
            entity.Property(e => e.NumberOfInfants).HasDefaultValue(0);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RemainingAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Tour).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__TourId__09A971A2");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__08B54D69");
        });

        modelBuilder.Entity<BookingExtraCharge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingE__3214EC0770CA08A8");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingExtraCharges)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingEx__Booki__1BC821DD");

            entity.HasOne(d => d.ExtraCharge).WithMany(p => p.BookingExtraCharges)
                .HasForeignKey(d => d.ExtraChargeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingEx__Extra__1CBC4616");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D3469906515E3");

            entity.Property(e => e.ContractId).ValueGeneratedNever();
            entity.Property(e => e.ContractFileUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedByTourOperator).HasDefaultValue(true);
            entity.Property(e => e.IsSignedByCustomer).HasDefaultValue(false);
            entity.Property(e => e.SignedAt).HasColumnType("datetime");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Booki__32AB8735");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__TourO__339FAB6E");
        });

        modelBuilder.Entity<DepartureDate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departur__3214EC07DA56838E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DepartureDate1).HasColumnName("DepartureDate");

            entity.HasOne(d => d.Tour).WithMany(p => p.DepartureDates)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departure__TourI__00200768");
        });

        modelBuilder.Entity<ExtraCharge>(entity =>
        {
            entity.HasKey(e => e.ExtraChargeId).HasName("PK__ExtraCha__23A8433163E6102E");

            entity.Property(e => e.ExtraChargeId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<GuideLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GuideLan__3214EC073D7D8576");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Guide).WithMany(p => p.GuideLanguages)
                .HasForeignKey(d => d.GuideId)
                .HasConstraintName("FK__GuideLang__Guide__5DCAEF64");

            entity.HasOne(d => d.Language).WithMany(p => p.GuideLanguages)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK__GuideLang__Langu__5EBF139D");
        });

        modelBuilder.Entity<GuideRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__GuideRat__FCCDF87C23B2F91C");

            entity.Property(e => e.RatingId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.TourGuide).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideRati__TourG__2BFE89A6");

            entity.HasOne(d => d.User).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuideRati__UserI__2CF2ADDF");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B93855ABC3CC294B");

            entity.Property(e => e.LanguageId).ValueGeneratedNever();
            entity.Property(e => e.LanguageName).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38336B94E0");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AmountPaid)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
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
                .HasConstraintName("FK__Payments__Bookin__40058253");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__41EDCAC5");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserId__40F9A68C");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentTypeId).HasName("PK__PaymentT__BA430B353FC67E2A");

            entity.Property(e => e.PaymentTypeId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PaymentTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<PurchaseTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Purchase__55433A6BA7168FAB");

            entity.Property(e => e.TransactionId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Package).WithMany(p => p.PurchaseTransactions)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseT__Packa__693CA210");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.PurchaseTransactions)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseT__TourO__68487DD7");
        });

        modelBuilder.Entity<PurchasedServicePackage>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBE338EF296");

            entity.Property(e => e.PurchaseId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NumOfToursUsed).HasDefaultValue(0);

            entity.HasOne(d => d.Package).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__Packa__6FE99F9F");

            entity.HasOne(d => d.TourOperator).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__TourO__6EF57B66");

            entity.HasOne(d => d.Transaction).WithMany(p => p.PurchasedServicePackages)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__Trans__70DDC3D8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ADAE76122");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160AA8BB858").IsUnique();

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SavedTour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SavedTou__3214EC0763387A86");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.SavedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tour).WithMany(p => p.SavedTours)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SavedTour__TourI__2180FB33");

            entity.HasOne(d => d.User).WithMany(p => p.SavedTours)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SavedTour__UserI__208CD6FA");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__ServiceP__322035CC8E4950CF");

            entity.Property(e => e.PackageId).ValueGeneratedNever();
            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DurationInYears).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("PK__Tours__604CEA3081C3056D");

            entity.Property(e => e.TourId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DurationInDays).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SlotsBooked).HasDefaultValue(0);
            entity.Property(e => e.StartPoint).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.TourStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.Transportation).HasMaxLength(255);

            entity.HasOne(d => d.TourOperator).WithMany(p => p.Tours)
                .HasForeignKey(d => d.TourOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tours__TourOpera__778AC167");
        });

        modelBuilder.Entity<TourCancellation>(entity =>
        {
            entity.HasKey(e => e.CancellationId).HasName("PK__TourCanc__6A2D9A3A541BFFB0");

            entity.Property(e => e.CancellationId).ValueGeneratedNever();
            entity.Property(e => e.CancelledAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.TourCancellations)
                .HasForeignKey(d => d.CancelledBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourCance__Cance__3864608B");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourCancellations)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourCance__TourI__37703C52");
        });

        modelBuilder.Entity<TourCompletionReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__TourComp__D5BD4805EDF8A396");

            entity.Property(e => e.ReportId).ValueGeneratedNever();
            entity.Property(e => e.AttachmentType).HasMaxLength(50);
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.ReportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalExtraCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.TourCompletionReports)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourCompl__Booki__46B27FE2");

            entity.HasOne(d => d.TourGuide).WithMany(p => p.TourCompletionReports)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourCompl__TourG__47A6A41B");
        });

        modelBuilder.Entity<TourExperience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourExpe__3214EC07E24EFC16");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Tour).WithMany(p => p.TourExperiences)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourExper__TourI__05D8E0BE");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity.HasKey(e => e.TourGuideId).HasName("PK__TourGuid__2F0E03537ACD676D");

            entity.HasIndex(e => e.UserId, "UQ__TourGuid__1788CC4D40106782").IsUnique();

            entity.Property(e => e.TourGuideId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.TourOperator).WithMany(p => p.TourGuides)
                .HasForeignKey(d => d.TourOperatorId)
                .HasConstraintName("FK__TourGuide__TourO__5812160E");

            entity.HasOne(d => d.User).WithOne(p => p.TourGuide)
                .HasForeignKey<TourGuide>(d => d.UserId)
                .HasConstraintName("FK__TourGuide__UserI__571DF1D5");
        });

        modelBuilder.Entity<TourGuideAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourGuid__3214EC071CAE5430");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssignedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsLeadGuide).HasDefaultValue(false);

            entity.HasOne(d => d.TourGuide).WithMany(p => p.TourGuideAssignments)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourGuide__TourG__7D439ABD");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourGuideAssignments)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourGuide__TourI__7C4F7684");
        });

        modelBuilder.Entity<TourImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourImag__3214EC079DC64CF5");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourImages)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourImage__TourI__02FC7413");
        });

        modelBuilder.Entity<TourOperator>(entity =>
        {
            entity.HasKey(e => e.TourOperatorId).HasName("PK__TourOper__776E46D942697525");

            entity.HasIndex(e => e.UserId, "UQ__TourOper__1788CC4DD9E06591").IsUnique();

            entity.Property(e => e.TourOperatorId).ValueGeneratedNever();
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.TourOperator)
                .HasForeignKey<TourOperator>(d => d.UserId)
                .HasConstraintName("FK__TourOpera__UserI__52593CB8");
        });

        modelBuilder.Entity<TourRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__TourRati__FCCDF87C7EC51D83");

            entity.Property(e => e.RatingId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Tour).WithMany(p => p.TourRatings)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourRatin__TourI__2645B050");

            entity.HasOne(d => d.User).WithMany(p => p.TourRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourRatin__UserI__2739D489");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE4A6A2AC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345EB09E15").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
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
                .HasConstraintName("FK__Users__RoleId__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

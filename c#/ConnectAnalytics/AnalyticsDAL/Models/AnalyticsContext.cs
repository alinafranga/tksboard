using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class AnalyticsContext : DbContext
    {
        public AnalyticsContext()
        {
        }

        public AnalyticsContext(DbContextOptions<AnalyticsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Follow> Follow { get; set; }
        public virtual DbSet<Impression> Impression { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<SummaryDay> SummaryDay { get; set; }
        public virtual DbSet<SummaryMonth> SummaryMonth { get; set; }
        public virtual DbSet<SummaryProductDay> SummaryProductDay { get; set; }
        public virtual DbSet<SummaryProductMonth> SummaryProductMonth { get; set; }
        public virtual DbSet<SummaryProductYear> SummaryProductYear { get; set; }
        public virtual DbSet<SummaryYear> SummaryYear { get; set; }
        public virtual DbSet<TempRp> TempRp { get; set; }
        public virtual DbSet<TempWp> TempWp { get; set; }
        public virtual DbSet<View> View { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MeasuredDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Impression>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MeasuredDate).HasColumnType("datetime");

                entity.Property(e => e.WeightedCount).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MeasuredDate).HasColumnType("datetime");

                entity.Property(e => e.WeightedCount).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.ProvinceAbbr)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<SummaryDay>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<SummaryProductDay>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<TempRp>(entity =>
            {
                entity.ToTable("TempRP");
            });

            modelBuilder.Entity<TempWp>(entity =>
            {
                entity.ToTable("TempWP");
            });

            modelBuilder.Entity<View>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MeasuredDate).HasColumnType("datetime");

                entity.Property(e => e.WeightedCount).HasDefaultValueSql("((1))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

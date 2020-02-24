using Microsoft.EntityFrameworkCore;
using XPenC.DataAccess.EntityFrameworkCore.Schema;

namespace XPenC.DataAccess.EntityFrameworkCore
{
    public class XPenCDbContext : DbContext
    {
        public XPenCDbContext(DbContextOptions<XPenCDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExpenseReportItemEntity> ExpenseReportItems { get; set; }
        public DbSet<ExpenseReportEntity> ExpenseReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseReportItemEntity>(entity =>
            {
                entity.HasKey(e => new { e.ExpenseReportId, e.ItemNumber });

                entity.HasIndex(e => e.Date);

                entity.HasIndex(e => e.ExpenseType);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.ExpenseType)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Value).HasColumnType("decimal(8, 4)");

                entity.HasOne<ExpenseReportEntity>()
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ExpenseReportId);
            });

            modelBuilder.Entity<ExpenseReportEntity>(entity =>
            {
                entity.HasIndex(e => e.Client);

                entity.HasIndex(e => e.ModifiedOn);

                entity.Property(e => e.Client)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MealTotal).HasColumnType("decimal(8, 4)");

                entity.Property(e => e.Total).HasColumnType("decimal(8, 4)");
            });
        }
    }
}

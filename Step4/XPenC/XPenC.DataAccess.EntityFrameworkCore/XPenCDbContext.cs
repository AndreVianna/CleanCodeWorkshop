using Microsoft.EntityFrameworkCore;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.EntityFrameworkCore
{
    public class XPenCDbContext : DbContext
    {
        public XPenCDbContext(DbContextOptions<XPenCDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExpenseReportEntity> ExpenseReports { get; set; }

        public DbSet<ExpenseReportItemEntity> ExpenseReportItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseReportEntity>(e =>
            {
                e.ToTable("ExpenseReports");
                e.HasKey(i => i.Id);
                e.Property(i => i.MealTotal).HasColumnType("decimal(8,4)");
                e.Property(i => i.Total).HasColumnType("decimal(8,4)");
                e.HasMany(i => i.Items)
                    .WithOne()
                    .HasPrincipalKey(i => i.Id)
                    .HasForeignKey(i => i.ExpenseReportId)
                    .HasConstraintName("FK_ExpenseReportItems_ExpenseReports")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExpenseReportItemEntity>(e =>
            {
                e.ToTable("ExpenseReportItems");
                e.HasKey(i => new { i.ExpenseReportId, i.ItemNumber });
                e.Property(i => i.Value).HasColumnType("decimal(8,4)");
            });
        }
    }
}
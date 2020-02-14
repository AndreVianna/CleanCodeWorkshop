using Microsoft.EntityFrameworkCore;
using XPenC.Database.Schema;

namespace XPenC.Database
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<ExpenseReportItemEntity> ExpenseReportItems { get; set; }

        public DbSet<ExpenseReportEntity> ExpenseReports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseReportItemEntity>(e =>
            {
                e.HasKey(i => new {InvoiceId = i.ExpenseReportId, i.ItemNumber });
                e.HasOne(i => i.ExpenseReport)
                    .WithMany(i => i.Items)
                    .OnDelete(DeleteBehavior.Cascade).IsRequired();
            });
        }
    }
}

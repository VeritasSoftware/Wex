using Microsoft.EntityFrameworkCore;
using Wex.API.Entities;

namespace Wex.API.Repositories
{
    public class MoneyManagementContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public MoneyManagementContext(DbContextOptions<MoneyManagementContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DateOnly mapping (EF Core 8 supports it directly)
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Date)
                .HasColumnType("date");

            base.OnModelCreating(modelBuilder);
        }
    }
}

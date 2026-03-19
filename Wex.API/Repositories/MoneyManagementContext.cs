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
            modelBuilder.Entity<Card>()
                .Property(t => t.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Card>()
                .Property(t => t.Identifier)
                .IsRequired(true);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Date)
                .IsRequired(true)
                .HasColumnType("date");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Identifier)
                .IsRequired(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}

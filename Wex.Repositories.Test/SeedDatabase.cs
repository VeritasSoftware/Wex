using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Wex.API.Models;
using Wex.API.Repositories;

namespace Wex.Repositories.Test
{
    public static class SeedDatabase
    {
        public static MoneyManagementContext SeedCardsDatabase() 
        {
            var options = new DbContextOptionsBuilder<MoneyManagementContext>()
            .UseSqlite("Data Source=cards.db") // SQLite for simplicity
            .Options;

            var context = new MoneyManagementContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Cards.Add(new Card
            {
                Id = 1,
                CreditLimit = 1000.50m
            });

            context.Transactions.Add(new Transaction
            {
                Id = 1,
                Amount = 100.00m,
                Date = DateOnly.FromDateTime(DateTime.Parse("16-03-2026", null, DateTimeStyles.AssumeLocal).ToUniversalTime()),
                Description = "Test Transaction",
                CardId = 1,
                Identifier = Guid.Parse("482acfc4-2aca-49b7-bfaa-14c92ad99d83")
            });

            context.SaveChanges();

            context.Transactions.Add(new Transaction
            {
                Id = 2,
                Amount = 200.00m,
                Date = DateOnly.FromDateTime(DateTime.Parse("17-03-2026", null, DateTimeStyles.AssumeLocal).ToUniversalTime()),
                Description = "Another Test Transaction",
                CardId = 1,
                Identifier = Guid.Parse("3d142c0f-1fc9-48cf-be15-26eea3497b71")
            });

            context.SaveChanges();

            return context;
        }
    }
}

using System.Globalization;
using Wex.API.Models;

namespace Wex.Repositories.Test
{
    public class TransactionsTestData : TheoryData<Guid, Transaction?>
    {
        public TransactionsTestData()
        {
            Add
            (
                Guid.Parse("3d142c0f-1fc9-48cf-be15-26eea3497b71"),
                new Transaction
                {
                    Id = 2,
                    Amount = 200.0m,
                    Date = DateOnly.FromDateTime(DateTime.Parse("17-03-2026", null, DateTimeStyles.AssumeLocal)),
                    Description = "Another Test Transaction",
                    CardId = 1,
                    Identifier = Guid.Parse("3d142c0f-1fc9-48cf-be15-26eea3497b71"),
                    Card = null
                }
            );
            Add
            (
                Guid.Parse("92876d62-a98a-4fd3-ae29-28cd12a1d5bd"),
                null
            );
        }
    }
}

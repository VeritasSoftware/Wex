using Wex.API.Models;
using Wex.API.Repositories;

namespace Wex.Repositories.Test
{    
    public class MoneyManagementRepositoryTests
    {
        private readonly MoneyManagementContext _context;

        public MoneyManagementRepositoryTests() 
        {            
            _context = SeedDatabase.SeedCardsDatabase();
        }        

        [Theory]
        [ClassData(typeof(TransactionsTestData))]
        public async Task GetTransaction_Success(Guid identifier, Transaction expectedResult)
        {
            // Arrange
            var repository = new MoneyManagementRepository(_context);

            // Act
            var transaction = await repository.GetTransactionAsync(identifier);

            // Assert
            Assert.Equal(transaction, expectedResult);
        }

        [Fact]
        public async Task AddTransactionAsync_Success()
        {
            // Arrange
            var repository = new MoneyManagementRepository(_context);

            var identifier = Guid.NewGuid();

            var transaction = new Transaction
            {
                Identifier = identifier,
                Amount = 100,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = "Test Transaction",
                CardId = 1
            };

            // Act
            await repository.AddTransactionAsync(transaction);

            var transactionFromDb = await repository.GetTransactionAsync(identifier);

            // Assert
            Assert.NotNull(transactionFromDb);
            Assert.Equal(transaction, transactionFromDb);
        }
    }
}

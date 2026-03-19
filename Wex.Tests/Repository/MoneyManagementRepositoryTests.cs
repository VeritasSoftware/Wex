using Microsoft.EntityFrameworkCore;
using Wex.API.Entities;
using Wex.API.Repositories;

namespace Wex.Tests.Repository
{    
    public class MoneyManagementRepositoryTests : IAsyncDisposable
    {
        private readonly MoneyManagementContext _context;

        public MoneyManagementRepositoryTests() 
        {            
            _context = SeedDatabase.SeedCardsDatabase();
        }        

        [Theory]
        [ClassData(typeof(TransactionsTestData))]
        public async Task GetTransactionAsync(Guid identifier, Transaction expectedResult)
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
                Date = DateTime.UtcNow,
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

        [Fact]
        public async Task AddTransactionAsync_ForeignKeyNotProvided_Fail()
        {
            // Arrange
            var repository = new MoneyManagementRepository(_context);

            var identifier = Guid.NewGuid();

            var transaction = new Transaction
            {
                Identifier = identifier,
                Amount = 100,
                Date = DateTime.UtcNow,
                Description = "Test Transaction",
            };

            // Act
            try
            {
                await repository.AddTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Equal("SQLite Error 19: 'FOREIGN KEY constraint failed'.", ex.InnerException?.Message);
                return;
            }            
        }

        [Fact]
        public async Task AddTransactionAsync_ForeignKeyDoesNotExist_Fail()
        {
            // Arrange
            var repository = new MoneyManagementRepository(_context);

            var identifier = Guid.NewGuid();

            var transaction = new Transaction
            {
                Identifier = identifier,
                Amount = 100,
                Date = DateTime.UtcNow,
                Description = "Test Transaction",
                CardId = 999 // Non-existent CardId
            };

            // Act
            try
            {
                await repository.AddTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Equal("SQLite Error 19: 'FOREIGN KEY constraint failed'.", ex.InnerException?.Message);
                return;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _context.Database.CloseConnectionAsync();

            await  _context.DisposeAsync();
        }
    }
}

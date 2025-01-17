using System;
using System.Linq;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace BookingRepositoryTests
{
    public class BookingRepositoryTests : IClassFixture<DependencyInjectionFixture>
    {
        private readonly AppDbContext _context;
        private readonly IBookingRepository _repository;

        public BookingRepositoryTests(DependencyInjectionFixture fixture)
        {
            var scope = fixture.ServiceProvider.CreateScope();
            _repository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Ledgers.AddRange(
                new Bank.Core.Models.Ledger { Id = 99, Balance = 1000m },
                new Bank.Core.Models.Ledger { Id = 100, Balance = 500m },
                new Bank.Core.Models.Ledger { Id = 101, Balance = 0m } // Destination ledger for specific tests
            );

            _context.SaveChanges();
        }

        [Fact]
        public void Book_ValidTransaction_UpdatesBalances()
        {
            // Act
            var result = _repository.Book(99, 100, 200m);

            // Assert
            Assert.True(result);

            var sourceLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 99);
            var destinationLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 100);

            Assert.Equal(800m, sourceLedger.Balance);
            Assert.Equal(700m, destinationLedger.Balance);
        }

        [Fact]
        public void Book_InsufficientFunds_ThrowsException()
        {
            SeedDatabase();
            // Assert
            Assert.Throws<InvalidOperationException>(() =>
                _repository.Book(100, 99, 1000m));
        }

        [Fact]
        public void Book_NegativeAmount_ThrowsArgumentException()
        {
            // Assert
            Assert.Throws<ArgumentException>(() =>
                _repository.Book(99, 100, -100m));
        }

        [Fact]
        public void Book_LedgerNotFound_ThrowsInvalidOperationException()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() =>
                _repository.Book(999, 99, 100m));
        }

        [Fact]
        public void Book_TransactionRollback_RetriesAndFailsAfterMaxAttempts()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Simulate a database error scenario

            // Act
            var result = _repository.Book(99, 100, 100m);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class Ledger
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }

    public class DatabaseSettings
    {
        // Add necessary database settings properties if needed
    }
}

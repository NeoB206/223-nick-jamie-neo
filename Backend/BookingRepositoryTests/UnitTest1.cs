using System;
using System.Linq;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace BookingRepositoryTests
{
    public class BookingRepositoryTests : IDisposable
    {
        private readonly Bank.DbAccess.Data.AppDbContext _context;
        private readonly BookingRepository _repository;

        public BookingRepositoryTests()
        {
            var connectionString = "Server=localhost;Database=bank;User=root;Password=020307;";

            var options = new DbContextOptionsBuilder<Bank.DbAccess.Data.AppDbContext>()
                .UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(11, 6, 2)),
                    sqlOptions => sqlOptions.MigrationsAssembly("Bank.Web")
                )
                .Options;

            _context = new Bank.DbAccess.Data.AppDbContext(options);

            var dbSettings = Options.Create(new Bank.DbAccess.DatabaseSettings
            {
                // Simulate configuration options if needed
                ConnectionString = connectionString
            });

            _repository = new BookingRepository(dbSettings, _context);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Ledgers.AddRange(
                new Bank.Core.Models.Ledger { Id = 1, Balance = 1000m },
                new Bank.Core.Models.Ledger { Id = 2, Balance = 500m },
                new Bank.Core.Models.Ledger { Id = 3, Balance = 0m } // Destination ledger for specific tests
            );

            _context.SaveChanges();
        }

        [Fact]
        public void Book_ValidTransaction_UpdatesBalances()
        {
            // Act
            var result = _repository.Book(1, 2, 200m);

            // Assert
            Assert.True(result);

            var sourceLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 1);
            var destinationLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 2);

            Assert.Equal(800m, sourceLedger.Balance);
            Assert.Equal(700m, destinationLedger.Balance);
        }

        [Fact]
        public void Book_InsufficientFunds_ThrowsException()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() =>
                _repository.Book(2, 1, 1000m));
        }

        [Fact]
        public void Book_NegativeAmount_ThrowsArgumentException()
        {
            // Assert
            Assert.Throws<ArgumentException>(() =>
                _repository.Book(1, 2, -100m));
        }

        [Fact]
        public void Book_LedgerNotFound_ThrowsInvalidOperationException()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() =>
                _repository.Book(999, 1, 100m));
        }

        [Fact]
        public void Book_TransactionRollback_RetriesAndFailsAfterMaxAttempts()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Simulate a database error scenario

            // Act
            var result = _repository.Book(1, 2, 100m);

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

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Ledger> Ledgers { get; set; }
    }
}

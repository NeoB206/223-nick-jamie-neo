﻿using System;
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
            _context.Ledgers.RemoveRange(_context.Ledgers);
            _context.Ledgers.AddRange(
                new Bank.Core.Models.Ledger { Id = 99, Balance = 1000m },
                new Bank.Core.Models.Ledger { Id = 100, Balance = 500m },
                new Bank.Core.Models.Ledger { Id = 101, Balance = 0m }
            );

            _context.SaveChanges();
        }

        [Fact]
        public void Book_ValidTransaction_UpdatesBalances()
        {
            var result = _repository.Book(99, 100, 200m);

            Assert.True(result);

            var sourceLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 99);
            var destinationLedger = _context.Ledgers.FirstOrDefault(l => l.Id == 100);

            Assert.Equal(800m, sourceLedger.Balance);
            Assert.Equal(700m, destinationLedger.Balance);
        }

        [Fact]
        public void Book_InsufficientFunds_ReturnsFalse()
        {
            Assert.False(_repository.Book(100, 99, 1000m));
        }

        [Fact]
        public void Book_NegativeAmount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _repository.Book(99, 100, -100m));
        }

        [Fact]
        public void Book_LedgerNotFound_ReturnsFalse()
        {
            Assert.False(_repository.Book(999, 99, 100m));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
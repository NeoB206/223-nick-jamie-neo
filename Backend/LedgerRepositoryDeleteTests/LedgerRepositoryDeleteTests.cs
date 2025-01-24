using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using BookingRepositoryTests;

namespace LedgerRepositoryDeleteTests;

public class LedgerRepositoryDeleteTests : IClassFixture<DependencyInjectionFixture>
{
    private readonly ILedgerRepository _ledgerRepository;
    private readonly AppDbContext _dbContext;

    public LedgerRepositoryDeleteTests(DependencyInjectionFixture fixture)
    {
        var scope = fixture.ServiceProvider.CreateScope();
        _ledgerRepository = scope.ServiceProvider.GetRequiredService<ILedgerRepository>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Ledgers.RemoveRange(_dbContext.Ledgers);
        _dbContext.Ledgers.AddRange(
            new Bank.Core.Models.Ledger { Id = 1, Balance = 1000m },
            new Bank.Core.Models.Ledger { Id = 2, Balance = 500m },
            new Bank.Core.Models.Ledger { Id = 3, Balance = 200m }
        );

        _dbContext.SaveChanges();
    }

    [Fact]
    public void Delete_ExistingLedger_RemovesLedger()
    {
        _ledgerRepository.Delete(1);

        var ledger = _dbContext.Ledgers.Find(1);
        Assert.Null(ledger);
    }

    [Fact]
    public void Delete_NonExistingLedger_ThrowsException()
    {
        var exception = Assert.Throws<Exception>(() => _ledgerRepository.Delete(999));
        Assert.Equal("Ledger with ID 999 does not exist.", exception.Message);
    }

    [Fact]
    public void Delete_TransactionRollback_OnFailure()
    {
        var existingLedger = _dbContext.Ledgers.Find(1);
        Assert.NotNull(existingLedger);

        try
        {
            _dbContext.Database.BeginTransaction();
            _ledgerRepository.Delete(999);
        }
        catch
        {
            var rolledBackLedger = _dbContext.Ledgers.Find(1);
            Assert.NotNull(rolledBackLedger);
        }
    }
}
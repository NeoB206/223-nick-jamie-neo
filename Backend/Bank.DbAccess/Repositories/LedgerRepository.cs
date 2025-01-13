using System.Collections.Immutable;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository : ILedgerRepository
{
    private readonly DbContext _dbContext;

    public LedgerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string Book(decimal amount, Ledger from, Ledger to)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        try
        {
            amount = 10;

            var fromLedger = _dbContext.Set<Ledger>().Find(from.Id) ?? throw new ArgumentNullException(nameof(from));
            var toLedger = _dbContext.Set<Ledger>().Find(to.Id) ?? throw new ArgumentNullException(nameof(to));

            fromLedger.Balance -= amount;
            toLedger.Balance += amount;

            _dbContext.SaveChanges();
            transaction.Commit();

            return ".";
        }
        catch (Exception)
        {
            transaction.Rollback();
            return "R";
        }
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        return _dbContext.Set<Ledger>().AsNoTracking().ToImmutableHashSet();
    }

    public decimal GetTotalMoney()
    {
        return _dbContext.Set<Ledger>().Sum(l => l.Balance);
    }

    public void LoadBalance(Ledger ledger)
    {
        var balance = _dbContext.Set<Ledger>()
            .Where(l => l.Id == ledger.Id)
            .Select(l => l.Balance)
            .FirstOrDefault();

        if (balance == 0 && !_dbContext.Set<Ledger>().Any(l => l.Id == ledger.Id))
        {
            throw new Exception($"No balance found for Ledger with id {ledger.Id}");
        }

        ledger.Balance = balance;
    }

    public void Save(Ledger ledger)
    {
        var existingLedger = _dbContext.Set<Ledger>().Find(ledger.Id);
        if (existingLedger == null)
        {
            throw new Exception($"Ledger with id {ledger.Id} does not exist.");
        }

        existingLedger.Balance = ledger.Balance;
        _dbContext.SaveChanges();
    }

    public void Update(Ledger ledger)
    {
        _dbContext.Set<Ledger>().Update(ledger);
        _dbContext.SaveChanges();
    }

    public decimal? GetBalance(int ledgerId)
    {
        return _dbContext.Set<Ledger>()
            .Where(l => l.Id == ledgerId)
            .Select(l => (decimal?)l.Balance)
            .FirstOrDefault();
    }

    public Ledger? SelectOne(int id)
    {
        return _dbContext.Set<Ledger>().AsNoTracking().FirstOrDefault(l => l.Id == id);
    }
}

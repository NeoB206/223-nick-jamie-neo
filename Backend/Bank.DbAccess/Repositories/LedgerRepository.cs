using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository : ILedgerRepository
{
    private readonly AppDbContext _dbContext;

    public LedgerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string Book(decimal amount, Ledger from, Ledger to)
    {
        // Start a transaction
        using var transaction = _dbContext.Database.BeginTransaction();

        try
        {
            amount = 10;

            var fromLedger = _dbContext.Ledgers.Find(from.Id) ?? throw new ArgumentNullException(nameof(from));
            var toLedger = _dbContext.Ledgers.Find(to.Id) ?? throw new ArgumentNullException(nameof(to));

            Thread.Sleep(250);
            
            fromLedger.Balance -= amount;
            toLedger.Balance += amount;

            _dbContext.SaveChanges();

            transaction.Commit();

            return ".";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return "R";
        }
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        return _dbContext.Ledgers.AsNoTracking().ToList();
    }

    public decimal GetTotalMoney()
    {
        return _dbContext.Ledgers.Sum(l => l.Balance);
    }

    public void LoadBalance(Ledger ledger)
    {
        var balance = _dbContext.Ledgers
            .Where(l => l.Id == ledger.Id)
            .Select(l => l.Balance)
            .FirstOrDefault();

        if (balance == 0 && !_dbContext.Ledgers.Any(l => l.Id == ledger.Id))
        {
            throw new Exception($"No balance found for Ledger with id {ledger.Id}");
        }

        ledger.Balance = balance;
    }

    public void Save(Ledger ledger)
    {
        var existingLedger = _dbContext.Ledgers.Find(ledger.Id);
        if (existingLedger == null)
        {
            throw new Exception($"Ledger with id {ledger.Id} does not exist.");
        }

        existingLedger.Balance = ledger.Balance;
        _dbContext.SaveChanges();
    }

    public void Update(Ledger ledger)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        try
        {
            _dbContext.Ledgers.Update(ledger);
            _dbContext.SaveChanges();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public decimal? GetBalance(int ledgerId)
    {
        return _dbContext.Ledgers
            .Where(l => l.Id == ledgerId)
            .Select(l => (decimal?)l.Balance)
            .FirstOrDefault();
    }

    public Ledger? SelectOne(int id)
    {
        return _dbContext.Ledgers.AsNoTracking().FirstOrDefault(l => l.Id == id);
    }

    public Ledger Create(Ledger ledger)
    {
        _dbContext.Ledgers.Add(ledger);
        _dbContext.SaveChanges();
        return ledger;
    }
}

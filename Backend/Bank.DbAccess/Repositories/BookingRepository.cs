using System.Transactions;
using Bank.DbAccess.Data;
using Microsoft.Extensions.Options;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    private readonly AppDbContext _context = context;

    public bool Book(int sourceLedgerId, int destinationLedgerId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var success = false;
        var repeatCount = 0;
        

        while (!success)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var sourceLedger = _context.Ledgers.FirstOrDefault(l => l.Id == sourceLedgerId);
                var destinationLedger = _context.Ledgers.FirstOrDefault(l => l.Id == destinationLedgerId);

                if (sourceLedger == null || destinationLedger == null)
                    throw new InvalidOperationException("One or both ledgers not found.");

                if (sourceLedger.Balance < amount)
                    throw new InvalidOperationException("Insufficient funds in source ledger.");

                sourceLedger.Balance -= amount;
                destinationLedger.Balance += amount;

                _context.SaveChanges();

                transaction.Commit();
                success = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                repeatCount++;
                if (repeatCount == 10)
                {
                    return false;
                }
            }
        }

        return success;
    }
}
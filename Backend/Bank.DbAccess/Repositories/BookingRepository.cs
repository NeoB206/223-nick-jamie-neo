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
                // Load source and destination accounts
                var sourceLedger = _context.Ledgers.FirstOrDefault(l => l.Id == sourceLedgerId);
                var destinationLedger = _context.Ledgers.FirstOrDefault(l => l.Id == destinationLedgerId);

                if (sourceLedger == null || destinationLedger == null)
                    throw new InvalidOperationException("One or both ledgers not found.");

                if (sourceLedger.Balance < amount)
                    throw new InvalidOperationException("Insufficient funds in source ledger.");

                // Perform the booking
                sourceLedger.Balance -= amount;
                destinationLedger.Balance += amount;

                // Save changes
                _context.SaveChanges();

                // Commit transaction
                transaction.Commit();
                success = true;
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                transaction.Rollback();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                repeatCount++;
                if (repeatCount == 10)
                {
                    return false;
                }
                // Retry logic: transaction will be retried unless a specific condition is met (e.g., max retries)
            }
        }

        return success;
    }
        // Machen Sie eine Connection und eine Transaktion

        // In der Transaktion:

        // Schauen Sie ob genügend Geld beim Spender da ist
        // Führen Sie die Buchung durch und UPDATEn Sie die ledgers
        // Beenden Sie die Transaktion
        // Bei einem Transaktionsproblem: Restarten Sie die Transaktion in einer Schleife 
        // (Siehe LedgersModel.SelectOne)

    //    return false; // Lösch mich
   // }
}
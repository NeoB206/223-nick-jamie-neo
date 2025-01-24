using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class Simple
{
    public static void Run(ILedgerRepository ledgerRepository)
    {
        var allLedgers = ledgerRepository.GetAllLedgers().ToArray();

        if (allLedgers.Length < 2)
        {
            Console.WriteLine("Insufficient ledgers for transactions.");
            return;
        }

        Console.WriteLine("Booking, press ESC to stop.");

        var random = new Random();
        while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
            try
            {
                var fromLedger = allLedgers[random.Next(allLedgers.Length)];
                var toLedger = allLedgers[random.Next(allLedgers.Length)];

                if (fromLedger.Id == toLedger.Id)
                {
                    continue;
                }

                var amount = random.Next(1, 101);

                ledgerRepository.Book(amount, fromLedger, toLedger);

                Console.WriteLine(
                    $"Booked transaction: {amount} from {fromLedger.Name} (ID: {fromLedger.Id}, Balance: {fromLedger.Balance}) to {toLedger.Name} (ID: {toLedger.Id}, Balance: {toLedger.Balance})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during booking: {ex.Message}");
            }
        }

        Console.WriteLine();

        Console.WriteLine("Getting total money in system at the end.");
        try
        {
            var startMoney = ledgerRepository.GetTotalMoney();
            Console.WriteLine($"Total end money: {startMoney}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in getting total money.");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("Hello, World!");
    }
}
using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class Simple
{
    public static void Run(ILedgerRepository ledgerRepository)
    {
        // Retrieve all ledgers and convert to an array
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
                // Choose random "from" and "to" accounts
                var fromLedger = allLedgers[random.Next(allLedgers.Length)];
                var toLedger = allLedgers[random.Next(allLedgers.Length)];

                // Ensure "from" and "to" accounts are not the same
                if (fromLedger.Id == toLedger.Id)
                {
                    continue;
                }

                // Generate a random transaction amount between 1 and 100
                var amount = random.Next(1, 101);

                // Book the transaction
                ledgerRepository.Book(amount, fromLedger, toLedger);

                // Print transaction details
                Console.WriteLine(
                    $"Booked transaction: {amount} from {fromLedger.Name} (ID: {fromLedger.Id}, Balance: {fromLedger.Balance}) to {toLedger.Name} (ID: {toLedger.Id}, Balance: {toLedger.Balance})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during booking: {ex.Message}");
            }
        }

        // Line break after exiting the loop
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
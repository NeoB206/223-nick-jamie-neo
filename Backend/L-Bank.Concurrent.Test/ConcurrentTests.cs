using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Bank.DbAccess.Repositories;
using Bank.DbAccess.Data;
using BookingRepositoryTests;

namespace L_Bank.Concurrent.Test;

public class ConcurrentTests : IClassFixture<DependencyInjectionFixture>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly AppDbContext _dbContext;

    public ConcurrentTests(DependencyInjectionFixture fixture)
    {
        // Resolve dependencies from the fixture's service provider
        var scope = fixture.ServiceProvider.CreateScope();
        _bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Seed data for tests
        const int totalLedgers = 100;
        _dbContext.Ledgers.RemoveRange(_dbContext.Ledgers);
        _dbContext.SaveChanges();

        for (int i = 0; i < totalLedgers; i++)
        {
            _dbContext.Ledgers.Add(new Bank.Core.Models.Ledger { Id = i + 1, Balance = 1000m });
        }

        _dbContext.SaveChanges();
    }

    [Fact]
    public void TestBookingParallel()
    {
        const int numberOfBookings = 1000;
        const int users = 10;

        Task[] tasks = new Task[users];

        void UserAction()
        {
            Random random = new Random();

            for (int i = 0; i < numberOfBookings; i++)
            {
                // Pick two random ledgers
                int sourceLedgerId = random.Next(1, 101);
                int destinationLedgerId;

                do
                {
                    destinationLedgerId = random.Next(1, 101);
                } while (destinationLedgerId == sourceLedgerId);

                decimal amount = random.Next(1, 100);

                try
                {
                    _bookingRepository.Book(sourceLedgerId, destinationLedgerId, amount);
                }
                catch (Exception ex)
                {
                    // Log exceptions if needed
                }
            }
        }

        for (int i = 0; i < users; i++)
        {
            tasks[i] = Task.Run(UserAction);
        }

        Task.WaitAll(tasks);

        // Output final balances for verification (optional)
        var ledgers = _dbContext.Ledgers.ToList();
        foreach (var ledger in ledgers)
        {
            Console.WriteLine($"Ledger {ledger.Id} final balance: {ledger.Balance}");
        }
    }
}

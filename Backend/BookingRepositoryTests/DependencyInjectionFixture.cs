using Bank.DbAccess;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace BookingRepositoryTests;

public class DependencyInjectionFixture : IDisposable
{
    
    public IServiceProvider ServiceProvider { get; }

    public DependencyInjectionFixture()
    {
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = "Server=localhost;Database=m223bank;User=root;Password=root;";
            // configuration.GetConnectionString("TestDatabase");

        services.AddDbContext<TestDbContext>(options =>
            options.UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(11, 6, 2))
            )
        );
        
        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        services.AddTransient<ILedgerRepository, LedgerRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IBookingRepository, BookingRepository>();

        ServiceProvider = services.BuildServiceProvider();

        // Ensure the database is created
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        // Cleanup database if needed
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        dbContext.Database.EnsureDeleted();
    }
    
    
}
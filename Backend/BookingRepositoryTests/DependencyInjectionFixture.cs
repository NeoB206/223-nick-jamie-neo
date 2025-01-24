using Bank.DbAccess;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


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

        var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(11, 6, 2))
            )
        );

        var dbSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() ??
                         throw new InvalidOperationException();
        var options = Options.Create(dbSettings);
        services.AddSingleton<IOptions<DatabaseSettings>>(options);


        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        services.AddTransient<ILedgerRepository, LedgerRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IBookingRepository, BookingRepository>();

        ServiceProvider = services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
        dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
    }
}
using Bank.Core.Models;
using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    // Add your DbSet properties here
    public DbSet<Booking> Bookings { get; set; }
    
    public DbSet<Ledger> Ledgers { get; set; }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure entities if necessary
    }
}
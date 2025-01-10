using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using Bank.Core.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository(IOptions<DatabaseSettings> databaseSettings) : ILedgerRepository
{
    private readonly DatabaseSettings _databaseSettings = databaseSettings.Value;

    public void oldBook(decimal amount, Ledger from, Ledger to)
    {
        LoadBalance(from);
        from.Balance -= amount;
        Save(from);
        // Complicate calculations
        Thread.Sleep(250);
        LoadBalance(to);
        to.Balance += amount;
        Save(to);
    }
    
    public void LoadBalance(Ledger ledger)
    {
        const string query = $"SELECT balance FROM {Ledger.CollectionName} WHERE id=@Id";

        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Id", ledger.Id);
        var result = cmd.ExecuteScalar();
        if (result != DBNull.Value)
        {
            ledger.Balance = Convert.ToDecimal(result);
        }
        else
        {
            throw new Exception($"No balance found for Ledger with id {ledger.Id}");
        }
    }

    public void Save(Ledger ledger)
    {
        const string query = $"UPDATE {Ledger.CollectionName} SET balance=@Balance WHERE id=@Id";

        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Balance", ledger.Balance);
        cmd.Parameters.AddWithValue("@Id", ledger.Id);
        cmd.ExecuteNonQuery();
    }
    
    public string Book(decimal amount, Ledger from, Ledger to)
    {
        using (MySqlConnection conn = new MySqlConnection(_databaseSettings.ConnectionString))
        {
            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    amount = 10;
                    from.Balance = this.GetBalance(from.Id, conn, transaction) ?? throw new ArgumentNullException();
                    from.Balance -= amount;
                    this.Update(from, conn, transaction);
                   // Complicate calculations
                    Thread.Sleep(250);
                    to.Balance = this.GetBalance(to.Id, conn, transaction) ?? throw new ArgumentNullException();
                    to.Balance += amount;
                    this.Update(to, conn, transaction);

                    // Console.WriteLine($"Booking {amount} from {from.Name} to {to.Name}");

                    transaction.Commit();
                    return ".";
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    //Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                        return "R";
                    }
                    catch (Exception ex2)
                    {
                        // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                        //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        //Console.WriteLine("  Message: {0}", ex2.Message);
                        return "E";
                    }
                }
            }
        }
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        var allLedgers = new HashSet<Ledger>();

        const string query = @$"SELECT id, name, balance FROM {Ledger.CollectionName}";
        using (SqlConnection conn = new SqlConnection(_databaseSettings.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        decimal balance = reader.GetDecimal(reader.GetOrdinal("balance"));

                        allLedgers.Add(new Ledger()
                        {
                            Balance = balance,
                            Id = id,
                            Name = name
                        });
                    }
                }
            }
        }

        return allLedgers.ToImmutableHashSet<Ledger>();
    }

    public decimal GetTotalMoney()
    {
        const string query = @$"SELECT SUM(balance) AS TotalBalance FROM {Ledger.CollectionName}";
        decimal totalBalance = 0;

        using (SqlConnection conn = new SqlConnection(_databaseSettings.ConnectionString))
        {
            conn.Open();
            using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            totalBalance = Convert.ToDecimal(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }

            return totalBalance;
        }
    }
    
    public Ledger? SelectOne(int id)
    {
        Ledger? retLedger = null;
        bool worked;

        do
        {
            worked = true;
            using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                retLedger = SelectOne(id, conn, transaction);
            }
            catch (Exception ex)
            {
                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                    if (ex.GetType() != typeof(Exception))
                        worked = false;
                }
                catch (Exception ex2)
                {
                    // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                    if (ex2.GetType() != typeof(Exception))
                        worked = false;
                }
            }
        } while (!worked);

        return retLedger;
    }

    public Ledger SelectOne(int id, MySqlConnection conn, MySqlTransaction? transaction)
    {
        const string query = $"SELECT id, name, balance FROM {Ledger.CollectionName} WHERE id=@Id";

        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            throw new Exception($"No Ledger with id {id}");

        var ordId = reader.GetInt32(reader.GetOrdinal("id"));
        var ordName = reader.GetString(reader.GetOrdinal("name"));
        var ordBalance = reader.GetDecimal(reader.GetOrdinal("balance"));

        return new Ledger { Id = ordId, Name = ordName, Balance = ordBalance };
    }

    public void Update(Ledger ledger, MySqlConnection conn, MySqlTransaction? transaction)
    {
        const string query = $"UPDATE {Ledger.CollectionName} SET name=@Name, balance=@Balance WHERE id=@Id";
        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Name", ledger.Name);
        cmd.Parameters.AddWithValue("@Balance", ledger.Balance);
        cmd.Parameters.AddWithValue("@Id", ledger.Id);
        
        cmd.ExecuteNonQuery();
    }

    public void Update(Ledger ledger)
    {
        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        Update(ledger, conn, null);
    }
    
    public decimal? GetBalance(int ledgerId, MySqlConnection conn, MySqlTransaction transaction)
    {
        const string query = "SELECT balance FROM ledgers WHERE id=@Id";

        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Id", ledgerId);
        var result = cmd.ExecuteScalar();
        if (result != DBNull.Value)
        {
            return Convert.ToDecimal(result);
        }

        return null;
    }
}
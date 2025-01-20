using Bank.Core.Models;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public interface ILedgerRepository
{
    IEnumerable<Ledger> GetAllLedgers();
    public string Book(decimal amount, Ledger from, Ledger to);
    decimal GetTotalMoney();
    Ledger? SelectOne(int id);
    void Update(Ledger ledger);
    decimal? GetBalance(int ledgerId);
    void Delete(int id);
    Ledger Create(Ledger ledger);
}
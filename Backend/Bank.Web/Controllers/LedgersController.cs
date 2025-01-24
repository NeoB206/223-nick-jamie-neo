using Bank.Core.Models;
using Bank.DbAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Web.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LedgersController(ILedgerRepository ledgerRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrators,Users")]
    public IEnumerable<Ledger> Get()
    {
        var allLedgers = ledgerRepository.GetAllLedgers();
        return allLedgers;
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrators,Users")]
    public Ledger? Get(int id)
    {
        var ledger = ledgerRepository.SelectOne(id);
        return ledger;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public void Put(int id, [FromBody] Ledger ledger)
    {
        ledgerRepository.Update(ledger);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Delete(int id)
    {
        try
        {
            ledgerRepository.Delete(id);
            return NoContent(); // Status 204
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"Ledger with ID {id} not found. Error: {ex.Message}" });
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public ActionResult<Ledger> Create([FromBody] Ledger ledger)
    {
        if (ledger.Balance < 0 || ledger.Name == null || ledger.Name == "")
        {
            return BadRequest("Ledger data is invalid.");
        }

        ledgerRepository.Create(ledger);
        return CreatedAtAction(nameof(Get), new { id = ledger.Id }, ledger);
    }
}

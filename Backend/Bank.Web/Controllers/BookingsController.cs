using Bank.Core.Models;
using Bank.DbAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank.Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController(IBookingRepository bookingRepository) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Booking booking)
    {
        return await Task.Run(() =>
        {
            IActionResult response;
            var bookingSuccess = bookingRepository.Book(booking.SourceId, booking.DestinationId, booking.Amount);

            if (bookingSuccess)
            {
                response = Ok(new { Message = "Booking successful" });
            }
            else
            {
                response = BadRequest(new { Message = "Booking failed. Please check the source account balance or try again later." });
            }

            return response;
        });
    }
}
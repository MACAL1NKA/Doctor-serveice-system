using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Data;
using ViewModels;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public BookingsController(AppointmentsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _context.Bookings.ToListAsync();

        // CPU => Cores => Threads

        return Ok(bookings);
    }

    // POST / bookings

    [HttpPost]



    public async Task<IActionResult> AddSingle(BookingViewModel viewModel)
    {
        
        var SlotTime = await _context.TimeSlots
            .Include(ts => ts.Schedule)
            .ThenInclude(s => s.Doctor)
            .Include(ts => ts.Bookings)
            .SingleOrDefaultAsync(ts => ts.Id == viewModel.TimeSlotId);
        
        var bookings = await _context.Bookings.CountAsync();
         
        if (SlotTime is null)
        {
            return BadRequest();
        }

        if (viewModel.AppointmentTime < DateTime.Today)
        {
            return BadRequest(" The date is bast");
        }

        if (viewModel.AppointmentTime.DayOfWeek != SlotTime.Schedule.Day)
        {
            return BadRequest(" Doctor is not available at the selected day");
        }
        if (SlotTime.MaxAppointments <= bookings )
        {
            return BadRequest(" This is not a valid");
        }



        var ticketPrice = SlotTime.Schedule.Doctor.TicketPrice;
        var rate = 0.02m;
        var commission = ticketPrice * rate;

        //TODO: real payment methords ZAAD Edahab

        // var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")!.Value;
        var ConvertionSuccess = int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) , out var userId);
        if(!ConvertionSuccess){
            return BadRequest("You are not registered yet");
        } 

        var transactionId = new Random().Next(10_000, 999_999);
        var booking = new Booking
        {
            TimeSlotId = SlotTime.Id,
            AppointmetTime = new DateTime(viewModel.AppointmentTime.Ticks, DateTimeKind.Utc),
            IsCompleted = false,
            UserId = userId,
            CreateAt = DateTime.UtcNow,
            TransactionId = transactionId,
            PaidAmount = 10,
            Commission = commission,
            DoctorRevenue = ticketPrice - commission,
            PaymentMethod = viewModel.PaymentMethod
        };

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        return Created("", booking);
    }
}

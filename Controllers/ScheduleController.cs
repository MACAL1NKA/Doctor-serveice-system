using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using ViewModels;
using Data;
using Models;
using Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]


public class SchedulesController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public SchedulesController(AppointmentsDbContext context)
    {
        _context = context;
    }

    //Get  
    [HttpGet]

    public async Task<IActionResult> GetMySchedules()
    {
       
        var schedule = await _context.Schedules
            .Include(s => s.TimeSlots)
            .Where(s => s.Doctor.UserId == User.GetId())
            .ToListAsync(HttpContext.RequestAborted);

        return Ok(schedule);
    }

    //POST / Schedule
    [HttpPost]
    public async Task<IActionResult> AddSchedule(ScheduleViewModel ViewModel)
    {
        
        var doctor = await _context.Doctors
        .Include(d => d.Schedule.Where(s => s.Day == ViewModel.Day))
            .SingleOrDefaultAsync(d => d.UserId == User.GetId() , HttpContext.RequestAborted);
        if (doctor is null)
        {
            return BadRequest("You are not registered yet");
        }
        if (!doctor.Isverified)
        {
            return BadRequest("You are not verified ");
        }

        if (doctor.Schedule.Any())
        {
            return BadRequest("!!");
        }

        var Schedule = new Schedule
        {
            Day = ViewModel.Day,
            Location = ViewModel.Location,
            DoctorId = doctor.Id, // TODO: this for loging information
            CreatedAt = DateTime.UtcNow,
            IsAvailable = true

        };

        _context.Schedules.Add(Schedule);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        
        return Created("", Schedule);
    }

    // PUT / schedule/ {id}

    [HttpPut("/schedule/{id}")]
    public async Task<IActionResult> UpdateSingle(int id, ModifyScheduleViewModel viewModel)
    {
        var Singleschedule = await _context.Schedules.FindAsync(id);
        if (Singleschedule is null)
        {
            return BadRequest($"Could not find schedule with id: {id}");
        }

        // TODO : only owner of the schedule is allowed to update   2

        Singleschedule.Location = viewModel.Location;
        Singleschedule.Day = viewModel.Day;
        Singleschedule.IsAvailable = viewModel.IsAvailable;



        _context.Schedules.Update(Singleschedule);
        await _context.SaveChangesAsync();

        return Ok();

    }

    // Get / {id / timeslots}
    [HttpPost("{id}/timeslots")]


    public async Task<IActionResult> AddTimeslot(int id, [FromBody] TimeSlotViewModel viewModel)
    {

        var schedule = await _context.Schedules.FindAsync(new object[] {id} , HttpContext.RequestAborted);

        if (schedule is null)
        {
            return BadRequest($" This {id} is not valid ");
        }



        var timeslot = new TimeSlot
        {
            StartTime = viewModel.StartTime,
            EndTime = viewModel.EndTime,
            Description = viewModel.Description,
            MaxAppointments = viewModel.MaxAppointments,
            ScheduleId = schedule.Id,
            CreateAt = DateTime.UtcNow

        };

        await _context.TimeSlots.AddAsync(timeslot , HttpContext.RequestAborted);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);

        return Created("", timeslot);
    }

    //PUT /schedule/timeslots/ {id}

    [HttpPut("timeslots/{id}")]

    public async Task<IActionResult> updateSingle(int id, TimeSlotViewModel timeslot)
    {
        var singleTimeSlot = await _context.TimeSlots.FindAsync(new object [] {id}  , cancellationToken: HttpContext.RequestAborted);

        if (singleTimeSlot is null)
        {
            return BadRequest($"No single time slot have id:{id}");
        }

        singleTimeSlot.StartTime = timeslot.StartTime;
        singleTimeSlot.EndTime = timeslot.EndTime;
        singleTimeSlot.Description = timeslot.Description;
        singleTimeSlot.MaxAppointments = timeslot.MaxAppointments;

        _context.TimeSlots.Update(singleTimeSlot);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);

        return Ok(singleTimeSlot);
    }



}

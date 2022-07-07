using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;




using Data;
using Models;
using ViewModels;
using Helpers;


namespace Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly AppointmentsDbContext _context;
    public DoctorsController(AppointmentsDbContext context)
    {
        _context = context;
    }

    //GET /doctors
    
    // TODO: add Filtering
    // Skip : page * size 
    // GET /doctors
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAll(int page, int size, string? phone)
	{
		// skip = page * size;

		var query = _context.Doctors
			.Include(d => d.User)
			.Skip(page * size)
			.Take(size);

		if (!string.IsNullOrEmpty(phone))
		{
			query = query.Where(d => d.Phone == phone);
		}

		var doctors = await query
			.OrderBy(d => d.Id)
			.ToListAsync(HttpContext.RequestAborted);

		return Ok(doctors);
	}

    //get / doctors / specialties 

    [HttpGet("specialties")]
    [AllowAnonymous]

    public async Task<IActionResult> GetSpecialties()
    {
        var specialties = await _context.Doctors
            .GroupBy(d => d.Specialty)
            .Select(g => new
            {
                Specialty = g.Key,
                Count = g.Count()
            })
            .ToListAsync(HttpContext.RequestAborted);

            return Ok(specialties);
    }


    //GET /doctors / 5

    [HttpGet("{id}", Name = nameof(GetSingle))]
    
    public async Task<IActionResult> GetSingle(int id)
    {
        var doctors = await _context.Doctors.Include(d => d.User)
            .SingleOrDefaultAsync(d => d.Id == User.GetId(), HttpContext.RequestAborted);
        if (doctors is null)
        {
            return NotFound(
                new
                {
                    message = "The reverence doctor is not found"
                }
            );
        }

        return Ok(doctors);
    }
    // Post /doctors
    [HttpPost]

    public async Task<IActionResult> Add([FromBody] DoctorViewModel doctorViewModel)
    {
        
        var doctor = await _context.Doctors
            .SingleOrDefaultAsync(d => d.UserId == User.GetId(), HttpContext.RequestAborted);
        if (doctor is not null)
        {
            return BadRequest(" You are all ready exist 😁");
        }


        doctor = new Doctor
        {
            Phone = doctorViewModel.Phone,
            Bio = doctorViewModel.Bio,
            Specialty = doctorViewModel.Specialty,
            Picture = doctorViewModel.Picture,
            Certicate = doctorViewModel.Certicate,
            TicketPrice = doctorViewModel.TicketPrice,
            CreateAt = DateTime.UtcNow,
            UserId = User.GetId(), // FIXME : userId is 0
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);

        return Created(nameof(GetSingle), doctor);
    }

    // PUT / doctors / 5
    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateSingle(int id, [FromBody] Doctor doctor)
    {
        var SingleDoctor = await _context.Doctors.FindAsync(id, HttpContext.RequestAborted);
        if (SingleDoctor is null)
        {
            return BadRequest();
        }

        SingleDoctor.Bio = doctor.Bio;
        SingleDoctor.User = doctor.User;
        SingleDoctor.Specialty = doctor.Specialty;
        SingleDoctor.Picture = doctor.Picture;
        SingleDoctor.Certicate = doctor.Certicate;
        SingleDoctor.TicketPrice = doctor.TicketPrice;

        _context.Doctors.Update(SingleDoctor);
        _context.SaveChanges();

        return NoContent();
    }

    // DELETE /doctors / 5

    [HttpDelete("{id}")]

    public async Task<IActionResult> DeleteSingle(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id, HttpContext.RequestAborted);

        if (doctor is null)
        {
            return BadRequest();
        }

        _context.Doctors.Remove(doctor);
        _context.SaveChanges();

        return NoContent();
    }

}

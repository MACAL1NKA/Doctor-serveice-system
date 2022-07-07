using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Controllers;

[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public UsersController(AppointmentsDbContext context)
    {
        _context = context;
    }
    // GET / Users
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _context.Users.OrderBy(u => u.Id).ToListAsync( HttpContext.RequestAborted);
        // var users = _context.Users.Where(u => u.Gender == "Male").ToList();
        return Ok(users);
    }

    // GET / Users / 5

    [HttpGet("{id}")]

    public async Task<IActionResult> FindUser(int id)
    {
        var user = await _context.Users.FindAsync(id, HttpContext.RequestAborted);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    //POST / Users
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] User user)
    {
        await _context.Users.AddAsync(user , HttpContext.RequestAborted);
        await _context.SaveChangesAsync( HttpContext.RequestAborted);

        return Created("", user);

    }

    // PUT / users / 5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser( int id ,[FromBody] User user)
    {
        var targetUser = await _context.Users.FindAsync(id);
        if (targetUser is null) 
        {
            return BadRequest();            
        }

        targetUser.FullName = user.FullName;
        targetUser.Email = user.Email;
        targetUser.Address = user.Address;
        targetUser.Gender = user.Gender;
        
       
       await _context.SaveChangesAsync(HttpContext.RequestAborted);

        return NoContent();
    }

    // DELETE / Users/ 5

    [HttpDelete("{id}")]

    public async Task<IActionResult> DELETE(int id)

    {

        var user =await _context.Users.FindAsync(id ,HttpContext.RequestAborted);
        if(user is null)
        {
            return BadRequest();
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);

        return NoContent();
    }
}

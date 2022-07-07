

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public AuthController(AppointmentsDbContext context)
    {
        _context = context;
    }

    // POST /Auth / loging
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string Email)
    {
        // TODO ; Validate user information

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == Email ,HttpContext.RequestAborted);

        if (user is null)
        {
            return BadRequest(" Invalid login attempt ");
        }

        var claims = new List<Claim>
        {

            new(JwtRegisteredClaimNames.Sub , user.Id.ToString()),
            new("FullName" , user.FullName),
            new("Email" , user.Email),
            new("Gendet", user.Gender)

        };

        

        var keyInput = "ranDom_text_with_at_least_32_charecTersticS";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.Now;
        var token = new JwtSecurityToken("My api", "My frontend", claims, now, now.AddHours(1), credentials);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);

        var result = new
        {
            toker = jwt
        };

        return Ok(result);
    }


}

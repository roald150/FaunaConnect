using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Controllers
{
    // Dit object gebruiken we specifiek voor het inloggen
    public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly FaunaDbContext _context;

        public UsersController(FaunaDbContext context)
        {
            _context = context;
        }

        // 1. POST: api/users/register (Account aanmaken)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            // Controleer of het e-mailadres al bestaat
            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
            {
                return BadRequest("Dit e-mailadres is al in gebruik.");
            }

            // In een productie-app zou je het wachtwoord hier hashen, 
            // maar voor dit assessment houden we het simpel en slaan we het direct op.
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

        // 2. POST: api/users/login (Inloggen)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Zoek de gebruiker in SQL Server
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized("Onjuist e-mailadres of wachtwoord.");
            }

            // Geef de gebruiker (inclusief zijn Rol!) terug naar de MAUI app
            return Ok(user);
        }
    }
}
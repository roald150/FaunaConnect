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
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized("Onjuist e-mailadres of wachtwoord.");
            }

            // Geef de gebruiker (inclusief zijn Rol!) terug naar de MAUI app
            return Ok(user);
        }

        // 3. GET: api/users (Alle gebruikers ophalen voor admin)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // 4. GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // 5. PUT: api/users/{id} (Gebruiker bewerken)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;
            user.LinkedJagerId = updatedUser.LinkedJagerId;

            // We raken het wachtwoord niet aan zoals gevraagd
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE: api/users/{id} (Gebruiker verwijderen)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
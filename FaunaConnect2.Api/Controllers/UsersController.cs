using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

// This object is used specifically for logging in
public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
public class LoginResponse { public string Token { get; set; } = ""; public User User { get; set; } = null!; }

[ApiController]
[Route("api/[controller]")]
public class UsersController(FaunaDbContext context, IConfiguration configuration) : ControllerBase
{
    // 1. POST: api/users/register (Create account)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        // Check if email address already exists
        if (await context.Users.AnyAsync(u => u.Email == newUser.Email))
        {
            return BadRequest("This email address is already in use.");
        }

        // Securely hash the password
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
        
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        return Ok(newUser);
    }

    // 2. POST: api/users/login (Login)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Incorrect email address or password.");
        }

        // Generate JWT Token
        var token = GenerateJwtToken(user);

        // Return the token and user info
        return Ok(new LoginResponse { Token = token, User = user });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // 3. GET: api/users (Retrieve all users for admin)
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }

    // 4. GET: api/users/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    // 5. PUT: api/users/{id} (Edit user)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        if (id != updatedUser.Id) return BadRequest();

        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;
        user.LinkedHunterId = updatedUser.LinkedHunterId;

        await context.SaveChangesAsync();
        return NoContent();
    }

    // 6. DELETE: api/users/{id} (Delete user)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

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
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The user data to register.</param>
    /// <returns>The registered user data.</returns>
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

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>A JWT token and user information.</returns>
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

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <remarks>
    /// Requires Admin role.
    /// </remarks>
    /// <returns>A list of all users.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user data.</returns>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    /// <summary>
    /// Updates a user's information.
    /// </summary>
    /// <remarks>
    /// Requires Admin role.
    /// </remarks>
    /// <param name="id">The user ID.</param>
    /// <param name="updatedUser">The updated user data.</param>
    /// <returns>No content.</returns>
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

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <remarks>
    /// Requires Admin role.
    /// </remarks>
    /// <param name="id">The user ID.</param>
    /// <returns>No content.</returns>
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

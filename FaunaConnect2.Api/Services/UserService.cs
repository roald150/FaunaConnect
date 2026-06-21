using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class UserService(FaunaDbContext context, IConfiguration configuration) : IUserService
{
    public async Task<List<UserResource>> GetAllAsync()
    {
        return await context.Users
            .Select(u => new UserResource
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                LinkedHunterId = u.LinkedHunterId,
                LinkedHunterName = u.LinkedHunter != null ? u.LinkedHunter.Name : null,
                RegistrationCount = u.Registrations.Count,
                DamageReportCount = u.DamageReports.Count
            })
            .ToListAsync();
    }

    public async Task<UserResource?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.LinkedHunter)
            .Where(u => u.Id == id)
            .Select(u => new UserResource
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                LinkedHunterId = u.LinkedHunterId,
                LinkedHunterName = u.LinkedHunter != null ? u.LinkedHunter.Name : null,
                RegistrationCount = u.Registrations.Count,
                DamageReportCount = u.DamageReports.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserResource> RegisterAsync(User user)
    {
        if (await context.Users.AnyAsync(u => u.Email == user.Email))
            throw new InvalidOperationException("This email address is already in use.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserResource
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            LinkedHunterId = user.LinkedHunterId
        };
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return new LoginResponse
        {
            Token = GenerateJwtToken(user),
            User = new UserResource
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                LinkedHunterId = user.LinkedHunterId
            }
        };
    }

    public async Task UpdateAsync(int id, User updatedUser)
    {
        var user = await context.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("User not found.");

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;
        user.LinkedHunterId = updatedUser.LinkedHunterId;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await context.Users.FindAsync(id)
            ?? throw new KeyNotFoundException("User not found.");

        context.Users.Remove(user);
        await context.SaveChangesAsync();
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
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}

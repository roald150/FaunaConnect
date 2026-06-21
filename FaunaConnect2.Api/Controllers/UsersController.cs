using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        try
        {
            var user = await userService.RegisterAsync(newUser);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await userService.LoginAsync(request.Email, request.Password);
        if (result == null)
            return Unauthorized("Incorrect email address or password.");

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResource>>> GetUsers()
    {
        return Ok(await userService.GetAllAsync());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResource>> GetUser(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        if (id != updatedUser.Id) return BadRequest();

        try
        {
            await userService.UpdateAsync(id, updatedUser);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DamageReportsController(FaunaDbContext context) : ControllerBase
{
    [Authorize(Roles = "Hunter,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DamageReport>>> Get()
    {
        return await context.DamageReports
            .Include(d => d.User)
            .OrderByDescending(d => d.Timestamp)
            .ToListAsync();
    }

    [Authorize(Roles = "Farmer,Admin")]
    [HttpPost]
    public async Task<ActionResult<DamageReport>> Create(DamageReport report)
    {
        report.Timestamp = DateTime.Now;
        context.DamageReports.Add(report);
        await context.SaveChangesAsync();
        return Ok(report);
    }
}

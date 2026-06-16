using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DamageReportsController : ControllerBase
{
    private readonly FaunaDbContext _context;

    public DamageReportsController(FaunaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DamageReport>>> Get()
    {
        return await _context.DamageReports
            .Include(d => d.User)
            .OrderByDescending(d => d.Timestamp)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<DamageReport>> Create(DamageReport report)
    {
        report.Timestamp = DateTime.Now;
        _context.DamageReports.Add(report);
        await _context.SaveChangesAsync();
        return Ok(report);
    }
}
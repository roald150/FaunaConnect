using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;
using FaunaConnect2.Api.Services;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DamageReportsController(IDamageReportService damageReportService) : ControllerBase
{
    [Authorize(Roles = "Hunter,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DamageReportResource>>> Get()
    {
        return Ok(await damageReportService.GetAllAsync());
    }

    [Authorize(Roles = "Farmer,Admin")]
    [HttpPost]
    public async Task<ActionResult<DamageReportResource>> Create(DamageReport report)
    {
        return Ok(await damageReportService.CreateAsync(report));
    }
}

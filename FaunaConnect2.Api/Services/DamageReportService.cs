using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class DamageReportService(FaunaDbContext context) : IDamageReportService
{
    public async Task<List<DamageReportResource>> GetAllAsync()
    {
        return await context.DamageReports
            .Include(d => d.User)
            .OrderByDescending(d => d.Timestamp)
            .Select(d => new DamageReportResource
            {
                Id = d.Id,
                Description = d.Description,
                Latitude = d.Latitude,
                Longitude = d.Longitude,
                PhotoUrl = d.PhotoUrl,
                Timestamp = d.Timestamp,
                UserId = d.UserId,
                UserName = d.User != null ? d.User.Name : null,
                HuntingGroundId = d.HuntingGroundId
            })
            .ToListAsync();
    }

    public async Task<DamageReportResource> CreateAsync(DamageReport report)
    {
        report.Timestamp = DateTime.Now;
        context.DamageReports.Add(report);
        await context.SaveChangesAsync();

        return new DamageReportResource
        {
            Id = report.Id,
            Description = report.Description,
            Latitude = report.Latitude,
            Longitude = report.Longitude,
            PhotoUrl = report.PhotoUrl,
            Timestamp = report.Timestamp,
            UserId = report.UserId,
            HuntingGroundId = report.HuntingGroundId
        };
    }
}

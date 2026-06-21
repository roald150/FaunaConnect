using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IDamageReportService
{
    Task<List<DamageReportResource>> GetAllAsync();
    Task<DamageReportResource> CreateAsync(Models.DamageReport report);
}

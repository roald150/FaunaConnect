using SQLite;
using FaunaConnect2.App.Models;

namespace FaunaConnect2.App.Services;

public class LocalDatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "FaunaConnect2Local.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<Registration>();
        await _database.CreateTableAsync<DamageReportDto>();
    }

    // --- Registration Methods ---
    public async Task<List<Registration>> GetUnsyncedRegistrationsAsync()
    {
        await Init();
        return await _database!.Table<Registration>().Where(r => !r.IsSynced).ToListAsync();
    }

    public async Task<int> SaveRegistrationAsync(Registration registration)
    {
        await Init();
        if (registration.Id != 0)
            return await _database!.UpdateAsync(registration);
        else
            return await _database!.InsertAsync(registration);
    }

    public async Task DeleteRegistrationAsync(Registration registration)
    {
        await Init();
        await _database!.DeleteAsync(registration);
    }

    // --- DamageReport Methods ---
    public async Task<List<DamageReportDto>> GetUnsyncedDamageReportsAsync()
    {
        await Init();
        return await _database!.Table<DamageReportDto>().Where(d => !d.IsSynced).ToListAsync();
    }

    public async Task<int> SaveDamageReportAsync(DamageReportDto report)
    {
        await Init();
        if (report.Id != 0)
            return await _database!.UpdateAsync(report);
        else
            return await _database!.InsertAsync(report);
    }

    public async Task DeleteDamageReportAsync(DamageReportDto report)
    {
        await Init();
        await _database!.DeleteAsync(report);
    }
}
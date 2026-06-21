using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class HuntingGroundService(FaunaDbContext context) : IHuntingGroundService
{
    public async Task<List<HuntingGroundResource>> GetAllAsync(int? userId)
    {
        var query = context.HuntingGrounds.AsQueryable();
        if (userId.HasValue)
            query = query.Where(h => h.UserId == userId.Value);

        return await query
            .Select(h => new HuntingGroundResource
            {
                Id = h.Id,
                Name = h.Name,
                TotalAreaInHectares = h.TotalAreaInHectares,
                PolygonCoordinatesJson = h.PolygonCoordinatesJson,
                UserId = h.UserId
            })
            .ToListAsync();
    }

    public async Task<HuntingGroundResource> CreateAsync(HuntingGround huntingGround)
    {
        context.HuntingGrounds.Add(huntingGround);
        await context.SaveChangesAsync();

        return new HuntingGroundResource
        {
            Id = huntingGround.Id,
            Name = huntingGround.Name,
            TotalAreaInHectares = huntingGround.TotalAreaInHectares,
            PolygonCoordinatesJson = huntingGround.PolygonCoordinatesJson,
            UserId = huntingGround.UserId
        };
    }

    public async Task DeleteAsync(int id)
    {
        var hg = await context.HuntingGrounds.FindAsync(id)
            ?? throw new KeyNotFoundException("Hunting ground not found.");
        context.HuntingGrounds.Remove(hg);
        await context.SaveChangesAsync();
    }
}

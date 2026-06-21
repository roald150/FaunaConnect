using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class AnimalSpeciesService(FaunaDbContext context) : IAnimalSpeciesService
{
    public async Task<List<AnimalSpeciesResource>> GetAllAsync()
    {
        return await context.AnimalSpecies
            .Select(s => new AnimalSpeciesResource
            {
                Id = s.Id,
                Name = s.Name,
                IconUrl = s.IconUrl
            })
            .ToListAsync();
    }

    public async Task<AnimalSpeciesResource> CreateAsync(AnimalSpecies species)
    {
        context.AnimalSpecies.Add(species);
        await context.SaveChangesAsync();

        return new AnimalSpeciesResource
        {
            Id = species.Id,
            Name = species.Name,
            IconUrl = species.IconUrl
        };
    }
}

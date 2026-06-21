using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IAnimalSpeciesService
{
    Task<List<AnimalSpeciesResource>> GetAllAsync();
    Task<AnimalSpeciesResource> CreateAsync(Models.AnimalSpecies species);
}

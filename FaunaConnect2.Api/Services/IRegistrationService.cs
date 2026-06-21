using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IRegistrationService
{
    Task<List<RegistrationResource>> GetAllAsync();
    Task<RegistrationResource> CreateAsync(Models.Registration registration);
}

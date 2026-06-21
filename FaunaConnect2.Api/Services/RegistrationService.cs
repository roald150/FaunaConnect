using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Data;
using FaunaConnect2.Api.Models;
using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public class RegistrationService(FaunaDbContext context) : IRegistrationService
{
    public async Task<List<RegistrationResource>> GetAllAsync()
    {
        return await context.Registrations
            .Include(r => r.User)
            .Select(r => new RegistrationResource
            {
                Id = r.Id,
                AnimalName = r.AnimalName,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                DateTime = r.DateTime,
                PhotoUrl = r.PhotoUrl,
                UserId = r.UserId,
                UserName = r.User != null ? r.User.Name : null
            })
            .ToListAsync();
    }

    public async Task<RegistrationResource> CreateAsync(Registration registration)
    {
        registration.DateTime = DateTime.Now;
        context.Registrations.Add(registration);
        await context.SaveChangesAsync();

        return new RegistrationResource
        {
            Id = registration.Id,
            AnimalName = registration.AnimalName,
            Latitude = registration.Latitude,
            Longitude = registration.Longitude,
            DateTime = registration.DateTime,
            PhotoUrl = registration.PhotoUrl,
            UserId = registration.UserId
        };
    }
}

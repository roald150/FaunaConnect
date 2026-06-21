using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IHuntingGroundService
{
    Task<List<HuntingGroundResource>> GetAllAsync(int? userId);
    Task<HuntingGroundResource> CreateAsync(Models.HuntingGround huntingGround);
    Task DeleteAsync(int id);
}

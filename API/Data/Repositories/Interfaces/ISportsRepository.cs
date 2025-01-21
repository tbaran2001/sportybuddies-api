using API.Modules.Sports.Models;

namespace API.Data.Repositories.Interfaces;

public interface ISportsRepository
{
    Task<IEnumerable<Sport>> GetAllSportsAsync();
    Task<Sport> GetSportByIdAsync(Guid sportId);
    Task AddSportAsync(Sport sport);
    void RemoveSport(Sport sport);
    Task<bool> SportNameExistsAsync(string sportName);
}
using API.Data.Repositories.Interfaces;
using API.Modules.Sports.Models;

namespace API.Data.Repositories.Implementations;

public class SportsRepository:ISportsRepository
{
    public async Task<IEnumerable<Sport>> GetAllSportsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Sport> GetSportByIdAsync(Guid sportId)
    {
        throw new NotImplementedException();
    }

    public async Task AddSportAsync(Sport sport)
    {
        throw new NotImplementedException();
    }

    public void RemoveSport(Sport sport)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SportNameExistsAsync(string sportName)
    {
        throw new NotImplementedException();
    }
}
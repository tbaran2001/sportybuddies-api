using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories.Implementations;

public class ProfilesRepository(ApplicationDbContext dbContext) : IProfilesRepository
{
    public async Task<Profile> GetProfileByIdAsync(Guid profileId)
    {
        return await dbContext.Profiles.FindAsync(profileId);
    }

    public async Task<Profile> GetProfileByIdWithSportsAsync(Guid profileId)
    {
        return await dbContext.Profiles
            .Include(u => u.Sports)
            .FirstOrDefaultAsync(u => u.Id == profileId);
    }

    public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
    {
        return await dbContext.Profiles
            .Include(p => p.Sports)
            .ToListAsync();
    }

    public async Task AddProfileAsync(Profile profile)
    {
        await dbContext.Profiles.AddAsync(profile);
    }

    public void RemoveProfile(Profile profile)
    {
        dbContext.Profiles.Remove(profile);
    }

    public async Task<IEnumerable<Profile>> GetPotentialMatchesAsync(Guid profileId, IEnumerable<Guid> profileSports)
    {
        return await dbContext.Profiles
            .Where(p => p.Id != profileId)
            .Where(p => p.Sports.Any(s => profileSports.Contains(s.Id)))
            .Where(p => !dbContext.Matches.Any(m =>
                (m.ProfileId == profileId && m.MatchedProfileId == p.Id) ||
                (m.ProfileId == p.Id && m.MatchedProfileId == profileId)))
            .ToListAsync();
    }
}
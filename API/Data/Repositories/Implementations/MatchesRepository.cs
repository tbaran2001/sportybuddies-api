using API.Data.Repositories.Interfaces;
using API.Modules.Matches.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories.Implementations;

public class MatchesRepository(ApplicationDbContext dbContext) : IMatchesRepository
{
    public async Task<Match> GetMatchByIdAsync(Guid matchId)
    {
        return await dbContext.Matches.FindAsync(matchId);
    }

    public async Task<IEnumerable<Match>> GetProfileMatchesAsync(Guid profileId)
    {
        return await dbContext.Matches
            .Where(m => m.ProfileId == profileId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetProfileExistingMatchesAsync(Guid profileId)
    {
        return await dbContext.Matches
            .Where(m => m.ProfileId == profileId || m.MatchedProfileId == profileId)
            .ToListAsync();
    }

    public async Task AddMatchesAsync(IEnumerable<Match> matches)
    {
        await dbContext.Matches.AddRangeAsync(matches);
    }

    public async Task<Match> GetRandomMatchAsync(Guid profileId)
    {
        var count = await dbContext.Matches.CountAsync(m => m.ProfileId == profileId && m.Swipe == null);
        if (count == 0)
            return null;

        var randomIndex = new Random().Next(count);

        return await dbContext.Matches
            .Where(m => m.ProfileId == profileId && m.Swipe == null)
            .Include(m => m.Profile)
            .Include(m => m.MatchedProfile)
            .ThenInclude(u => u.Sports)
            .Skip(randomIndex)
            .Take(1)
            .FirstOrDefaultAsync();
    }

    public Task RemoveRangeAsync(IEnumerable<Match> matches)
    {
        dbContext.Matches.RemoveRange(matches);

        return Task.CompletedTask;
    }

    public async Task RemoveInvalidMatchesForProfileAsync(Guid profileId)
    {
        await dbContext.Matches
            .Where(m => (m.ProfileId == profileId || m.MatchedProfileId == profileId) &&
                        !dbContext.Profiles
                            .Where(u => u.Id == m.ProfileId)
                            .SelectMany(u => u.Sports)
                            .Select(s => s.Id)
                            .Intersect(
                                dbContext.Profiles
                                    .Where(u => u.Id == m.MatchedProfileId)
                                    .SelectMany(u => u.Sports)
                                    .Select(s => s.Id)
                            ).Any()
            )
            .ExecuteDeleteAsync();
    }

    public async Task<bool> CheckIfMatchExistsAsync(Guid profileId, Guid matchedProfileId)
    {
        return await dbContext.Matches
            .AnyAsync(m => m.ProfileId == profileId && m.MatchedProfileId == matchedProfileId ||
                           m.ProfileId == matchedProfileId && m.MatchedProfileId == profileId);
    }

    public async Task<IEnumerable<Match>> GetActiveProfileMatchesAsync(Guid profileId)
    {
        return await dbContext.Matches
            .Where(m => m.ProfileId == profileId && m.Swipe == null)
            .ToListAsync();
    }
}
using API.Common.Models;
using API.Data.Repositories.Interfaces;
using API.Modules.Matches.Models;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class MatchService(
    IProfilesRepository profilesRepository,
    IMatchesRepository matchesRepository,
    IUnitOfWork unitOfWork)
    : IMatchService
{
    public async Task FindMatchesToAddAsync(Guid profileId)
    {
        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(profileId);
        if (profile == null)
            throw new Exception("Profile not found");
        if (profile.Sports.Count == 0)
            return;

        var potentialMatches =
            await profilesRepository.GetPotentialMatchesAsync(profileId, profile.Sports.Select(s => s.Id));

        var newMatches = new List<Match>();

        foreach (var matchedProfile in potentialMatches)
        {
            var (match1, match2) = Match.CreatePair(profileId, matchedProfile.Id, DateTime.UtcNow);
            newMatches.Add(match1);
            newMatches.Add(match2);
        }

        await matchesRepository.AddMatchesAsync(newMatches);
        await unitOfWork.CommitChangesAsync();
    }

    public async Task FindMatchesToRemoveAsync(Guid profileId)
    {
        await matchesRepository.RemoveInvalidMatchesForProfileAsync(profileId);
    }
}
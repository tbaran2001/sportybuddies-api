using API.Modules.Buddies.Models;

namespace API.Data.Repositories.Interfaces;

public interface IBuddiesRepository
{
    Task<IEnumerable<Buddy>> GetProfileBuddiesAsync(Guid profileId);
    Task AddBuddyAsync(Buddy buddy);
    Task<bool> AreProfilesAlreadyBuddiesAsync(Guid profileId, Guid matchedProfileId);
    Task RemoveProfileBuddiesAsync(Guid profileId);
    Task<IEnumerable<Buddy>> GetRelatedBuddies(Guid profileId, Guid matchedProfileId);
}
using API.Modules.Profiles.Models;

namespace API.Data.Repositories.Interfaces;

public interface IProfilesRepository
{
    Task<Profile> GetProfileByIdAsync(Guid profileId);
    Task<Profile> GetProfileByIdWithSportsAsync(Guid profileId);
    Task<IEnumerable<Profile>> GetAllProfilesAsync();
    Task AddProfileAsync(Profile profile);
    void RemoveProfile(Profile profile);
    Task<IEnumerable<Guid>> GetPotentialMatchesAsync(Guid profileId, IEnumerable<Guid> profileSports);
}
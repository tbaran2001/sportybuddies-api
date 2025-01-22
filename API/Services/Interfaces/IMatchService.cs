using API.Modules.Matches.Dtos;

namespace API.Services.Interfaces;

public interface IMatchService
{
    Task FindMatchesToAddAsync(Guid profileId);
    Task FindMatchesToRemoveAsync(Guid profileId);
    Task<MatchDto> GetRandomMatchAsync(Guid profileId);
}
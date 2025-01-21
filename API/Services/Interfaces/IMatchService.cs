namespace API.Services.Interfaces;

public interface IMatchService
{
    Task FindMatchesToAddAsync(Guid profileId);
    Task FindMatchesToRemoveAsync(Guid profileId);
}
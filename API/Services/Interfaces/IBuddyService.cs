using API.Modules.Matches.Models;

namespace API.Services.Interfaces;

public interface IBuddyService
{
    Task AddBuddyAsync(Match match, Match oppositeMatch);
}
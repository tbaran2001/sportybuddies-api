using API.Modules.Profiles.Features.Commands.RemoveProfileSport;
using API.Services.Interfaces;
using MassTransit;

namespace API.Modules.Matches.EventHandlers;

public class ProfileSportRemovedEventConsumer(IMatchService matchingService) : IConsumer<ProfileSportRemovedEvent>
{
    public async Task Consume(ConsumeContext<ProfileSportRemovedEvent> context)
    {
        await matchingService.FindMatchesToRemoveAsync(context.Message.ProfileId);
    }
}
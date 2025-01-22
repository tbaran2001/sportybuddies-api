using API.Modules.Profiles.Features.Commands.AddProfileSport;
using API.Services.Interfaces;
using MassTransit;

namespace API.Modules.Matches.EventHandlers;

public class ProfileSportAddedEventConsumer(IMatchService matchingService) : IConsumer<ProfileSportAddedEvent>
{
    public async Task Consume(ConsumeContext<ProfileSportAddedEvent> context)
    {
        await matchingService.FindMatchesToAddAsync(context.Message.ProfileId);
    }
}
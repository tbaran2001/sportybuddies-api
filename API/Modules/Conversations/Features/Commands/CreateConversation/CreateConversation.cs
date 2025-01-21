using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Modules.Conversations.Dtos;
using API.Services.Interfaces;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Conversations.Features.Commands.CreateConversation;

public record CreateConversationCommand(Guid ParticipantId) : ICommand<CreateConversationResult>;

public record CreateConversationResult(ConversationDto Conversation);

public record CreateConversationRequestDto(Guid ParticipantId);

public record CreateConversationResponseDto(ConversationDto Conversation);

public class CreateConversationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/conversations", async (CreateConversationRequestDto request, ISender sender) =>
            {
                var command = request.Adapt<CreateConversationCommand>();

                var result = await sender.Send(command);

                var response = result.Conversation.Adapt<CreateConversationResponseDto>();

                return Results.CreatedAtRoute("GetConversationById", new { id = response.Conversation.Id }, response);
            })
            .RequireAuthorization()
            .WithTags("Conversations")
            .WithName("CreateConversation")
            .Produces<CreateConversationResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a conversation")
            .WithDescription("Create a conversation");
    }
}

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.ParticipantId).NotEmpty();
    }
}

internal class CreateConversationCommandHandler(
    IConversationService conversationService,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateConversationCommand, CreateConversationResult>
{
    public async Task<CreateConversationResult> Handle(CreateConversationCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var conversation = await conversationService.CreateConversationAsync(currentUserId, command.ParticipantId);
        await unitOfWork.CommitChangesAsync();

        var conversationDto = conversation.Adapt<ConversationDto>();

        return new CreateConversationResult(conversationDto);
    }
}
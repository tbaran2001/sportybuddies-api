using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Hubs;
using API.Modules.Conversations.Dtos;
using API.Modules.Conversations.Exceptions;
using API.Modules.Conversations.Models;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Conversations.Features.Commands.SendMessage;

public record SendMessageCommand(Guid ConversationId, string Content) : ICommand<SendMessageResult>;

public record SendMessageResult(MessageDto Message);

public record SendMessageRequestDto(string Content);

public record SendMessageResponseDto(MessageDto Message);

public class SendMessageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/conversations/{conversationId:guid}/messages",
                async (Guid conversationId, SendMessageRequestDto request, ISender sender) =>
                {
                    var command = new SendMessageCommand(conversationId, request.Content);

                    var result = await sender.Send(command);

                    var response = result.Adapt<SendMessageResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization()
            .WithTags("Conversations")
            .WithName("SendMessage")
            .Produces<SendMessageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send a message to a conversation")
            .WithDescription("Send a message to a conversation by conversation id");
    }
}

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Content).NotNull().NotEmpty().MaximumLength(500);
    }
}

internal class SendMessageCommandHandler(
    IConversationsRepository conversationsRepository,
    IHubContext<ChatHub, IChatClient> hubContext,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
    : ICommandHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var conversation = await conversationsRepository.GetConversationByIdAsync(command.ConversationId);
        if (conversation is null)
            throw new ConversationNotFoundException(command.ConversationId);

        var message = Message.Create(conversation.Id, currentUserId, command.Content);

        await conversationsRepository.AddMessageAsync(message);
        await unitOfWork.CommitChangesAsync();

        await hubContext.Clients.Users(conversation.Participants.Select(p => p.ProfileId.ToString()))
            .ReceiveMessage(new HubMessage(message.Id, conversation.Id, message.SenderId, message.Content,
                message.CreatedOn,
                conversation.Participants.Select(p => p.ProfileId).ToList()));

        var messageDto = message.Adapt<MessageDto>();

        return new SendMessageResult(messageDto);
    }
}
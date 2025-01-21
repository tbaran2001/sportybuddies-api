﻿using API.Common.CQRS;
using API.Data.Repositories.Interfaces;
using API.Modules.Conversations.Dtos;
using API.Modules.Conversations.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Conversations.Features.Queries.GetConversationMessages;

public record GetConversationMessagesQuery(Guid ConversationId) : IQuery<GetConversationMessagesResult>;

public record GetConversationMessagesResult(IEnumerable<MessageDto> Messages);

public record GetConversationMessagesResponseDto(IEnumerable<MessageDto> Messages);

public class GetConversationMessagesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/conversations/{conversationId:guid}/messages",
                async (Guid conversationId, ISender sender) =>
                {
                    var query = new GetConversationMessagesQuery(conversationId);

                    var result = await sender.Send(query);

                    var response = result.Messages.Adapt<GetConversationMessagesResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization()
            .WithTags("Conversations")
            .WithName("GetConversationMessages")
            .Produces<GetConversationMessagesResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get messages for a conversation")
            .WithDescription("Get all messages for a conversation by conversation id");
    }
}

public class GetConversationMessagesQueryValidator : AbstractValidator<GetConversationMessagesQuery>
{
    public GetConversationMessagesQueryValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}

internal class GetConversationMessagesQueryHandler(IConversationsRepository conversationsRepository)
    : IQueryHandler<GetConversationMessagesQuery, GetConversationMessagesResult>
{
    public async Task<GetConversationMessagesResult> Handle(GetConversationMessagesQuery query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var conversation = await conversationsRepository.GetConversationByIdWithMessagesAsync(query.ConversationId);
        if (conversation is null)
            throw new ConversationNotFoundException(query.ConversationId);

        var messageDtos = conversation.Messages.Adapt<IEnumerable<MessageDto>>();

        return new GetConversationMessagesResult(messageDtos);
    }
}
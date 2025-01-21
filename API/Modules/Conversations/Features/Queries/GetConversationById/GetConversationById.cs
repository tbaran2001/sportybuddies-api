using API.Common.CQRS;
using API.Data.Repositories.Interfaces;
using API.Modules.Conversations.Dtos;
using API.Modules.Conversations.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Conversations.Features.Queries.GetConversationById;

public record GetConversationByIdQuery(Guid ConversationId) : IQuery<GetConversationByIdResult>;

public record GetConversationByIdResult(ConversationDto Conversation);

public record GetConversationByIdResponseDto(ConversationDto Conversation);

public class GetConversationByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/conversations/{conversationId:guid}", async (Guid conversationId, ISender sender) =>
            {
                var query = new GetConversationByIdQuery(conversationId);

                var result = await sender.Send(query);

                var response = result.Adapt<GetConversationByIdResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Conversations")
            .WithName("GetConversationById")
            .Produces<GetConversationByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get conversation by id")
            .WithDescription("Get a conversation by conversation id");
    }
}

public class GetConversationByIdQueryValidator : AbstractValidator<GetConversationByIdQuery>
{
    public GetConversationByIdQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty();
    }
}

internal class GetConversationByIdQueryHandler(IConversationsRepository conversationsRepository)
    : IQueryHandler<GetConversationByIdQuery, GetConversationByIdResult>
{
    public async Task<GetConversationByIdResult> Handle(GetConversationByIdQuery query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var conversation = await conversationsRepository.GetConversationByIdAsync(query.ConversationId);
        if (conversation is null)
            throw new ConversationNotFoundException(query.ConversationId);

        var conversationDto = conversation.Adapt<ConversationDto>();

        return new GetConversationByIdResult(conversationDto);
    }
}
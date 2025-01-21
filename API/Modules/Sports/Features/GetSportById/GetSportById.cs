using API.Common.CQRS;
using API.Data.Repositories.Interfaces;
using API.Modules.Sports.Dtos;
using API.Modules.Sports.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using Mapster;
using MediatR;

namespace API.Modules.Sports.Features.GetSportById;

public record GetSportByIdQuery(Guid Id) : IQuery<GetSportByIdResult>;

public record GetSportByIdResult(SportDto Sport);

public record GetSportByIdResponseDto(SportDto Sport);

public class GetSportById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/sports/{id}", async (Guid id, ISender sender) =>
            {
                var query = new GetSportByIdQuery(id);

                var result = await sender.Send(query);

                var response = result.Adapt<GetSportByIdResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Sports")
            .WithName("GetSportById")
            .Produces<GetSportByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get a sport by id")
            .WithDescription("Get a sport by id");
    }
}

internal class GetSportByIdQueryHandler(ISportsRepository sportsRepository)
    : IQueryHandler<GetSportByIdQuery, GetSportByIdResult>
{
    public async Task<GetSportByIdResult> Handle(GetSportByIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var sport = await sportsRepository.GetSportByIdAsync(query.Id);
        if (sport is null)
            throw new SportNotFoundException(query.Id);

        var sportDto = sport.Adapt<SportDto>();

        return new GetSportByIdResult(sportDto);
    }
}
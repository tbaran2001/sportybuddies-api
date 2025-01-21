﻿using API.Common.CQRS;
using API.Common.Models;
using API.Data.Repositories.Interfaces;
using API.Modules.Sports.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Sports.Features.CreateSport;

public record CreateSportCommand(string Name, string Description) : ICommand<CreateSportResult>;

public record CreateSportResult(Guid Id);

public record SportCreatedDomainEvent(Guid Id, string Name, string Description) : IDomainEvent;

public record CreateSportRequestDto(string Name, string Description);

public record CreateSportResponseDto(Guid Id);

public class CreateSportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/sports", async (CreateSportRequestDto request, IMediator mediator) =>
            {
                var command = request.Adapt<CreateSportCommand>();

                var result = await mediator.Send(command);

                var response = result.Adapt<CreateSportResponseDto>();

                return Results.CreatedAtRoute("GetSportById", new { id = result.Id }, response);
            })
            .RequireAuthorization()
            .WithTags("Sports")
            .WithName("CreateSport")
            .Produces<CreateSportResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new sport")
            .WithDescription("Create a new sport");
    }
}

public class CreateSportCommandValidator : AbstractValidator<CreateSportCommand>
{
    public CreateSportCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(500);
    }
}

internal class CreateSportCommandHandler(ISportsRepository sportsRepository)
    : ICommandHandler<CreateSportCommand, CreateSportResult>
{
    public async Task<CreateSportResult> Handle(CreateSportCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        if (await sportsRepository.SportNameExistsAsync(command.Name))
            throw new SportAlreadyExistException(command.Name);

        var sportEntity = Models.Sport.Create(command.Name, command.Description);

        await sportsRepository.AddSportAsync(sportEntity);

        return new CreateSportResult(sportEntity.Id);
    }
}
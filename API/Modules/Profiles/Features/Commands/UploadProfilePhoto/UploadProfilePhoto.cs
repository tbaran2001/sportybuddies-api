using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Exceptions;
using API.Services.Interfaces;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Commands.UploadProfilePhoto;

public record UploadProfilePhotoCommand(Stream File, string FileName) : ICommand<UploadProfilePhotoResult>;

public record UploadProfilePhotoResult(string PhotoUrl);

public record UploadProfilePhotoResponseDto(string PhotoUrl);

public class UploadProfilePhotoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profiles/upload-profile-photo", async (IFormFile file, ISender sender) =>
            {
                await using var stream = file.OpenReadStream();

                var command = new UploadProfilePhotoCommand(stream, file.FileName);

                var result = await sender.Send(command);

                var response = result.Adapt<UploadProfilePhotoResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("UploadProfilePhoto")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload a profile photo")
            .WithDescription("Upload a photo for the profile of the current user")
            .DisableAntiforgery();
    }
}

public class UploadProfilePhotoCommandValidator : AbstractValidator<UploadProfilePhotoCommand>
{
    public UploadProfilePhotoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull();
        RuleFor(x => x.FileName)
            .NotEmpty();
    }
}

internal class UploadProfilePhotoCommandHandler(
    IProfilesRepository profilesRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork,
    IProfilePhotoService profilePhotoService) : ICommandHandler<UploadProfilePhotoCommand, UploadProfilePhotoResult>
{
    public async Task<UploadProfilePhotoResult> Handle(UploadProfilePhotoCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();
        var profile = await profilesRepository.GetProfileByIdAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var photoUrl =
            await profilePhotoService.UploadAndAssignPhotoAsync(profile, command.File, command.FileName, true);
        await unitOfWork.CommitChangesAsync();

        return new UploadProfilePhotoResult(photoUrl);
    }
}
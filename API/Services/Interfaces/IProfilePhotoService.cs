using API.Modules.Profiles.Models;

namespace API.Services.Interfaces;

public interface IProfilePhotoService
{
    Task<string> UploadAndAssignPhotoAsync(Profile profile, Stream file, string fileName, bool isMain);
}
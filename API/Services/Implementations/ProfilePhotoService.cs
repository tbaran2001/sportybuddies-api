using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Models;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ProfilePhotoService(IBlobStorageService blobStorageService, IProfilesRepository iProfilesRepository)
    : IProfilePhotoService
{
    public async Task<string> UploadAndAssignPhotoAsync(Profile profile, Stream file, string fileName, bool isMain)
    {
        if (isMain && profile.MainPhotoUrl != null)
        {
            var response = await blobStorageService.DeleteBlobAsync(profile.MainPhotoUrl);
            if (!response)
                throw new Exception("Error deleting main photo");
            profile.RemoveMainPhoto();
        }

        var userFolder = profile.Id.ToString();
        var fullFileName = $"{userFolder}/{fileName}";

        var photoUrl = await blobStorageService.UploadToBlobAsync(file, fullFileName);

        profile.AddMainPhoto(photoUrl);

        return photoUrl;
    }
}
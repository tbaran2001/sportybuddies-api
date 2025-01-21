namespace API.Services.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadToBlobAsync(Stream file, string fileName);
    Task<bool> DeleteBlobAsync(string blobUrl);
    string GetBlobSasUrl(string blobUrl);
}
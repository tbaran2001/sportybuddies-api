using API.Services.Interfaces;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace API.Services.Implementations;

public class BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions) : IBlobStorageService
{
    private readonly BlobStorageSettings _blobStorageSettings = blobStorageSettingsOptions.Value;

    public async Task<string> UploadToBlobAsync(Stream file, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ProfilePicturesContainer);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(file, true);

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteBlobAsync(string blobUrl)
    {
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ProfilePicturesContainer);

        var fileName = GetBlobNameFromUrl(blobUrl);
        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DeleteIfExistsAsync();

        return response.Value;
    }

    public string GetBlobSasUrl(string blobUrl)
    {
        if (blobUrl == null) return null;

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobStorageSettings.ProfilePicturesContainer,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
            BlobName = GetBlobNameFromUrl(blobUrl)
        };


        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);


        var sasToken = sasBuilder
            .ToSasQueryParameters(new StorageSharedKeyCredential(blobServiceClient.AccountName,
                _blobStorageSettings.AccountKey))
            .ToString();

        return $"{blobUrl}?{sasToken}";
    }

    private string GetBlobNameFromUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        var segments = uri.Segments;
        return segments[^2] + segments[^1];
    }
}
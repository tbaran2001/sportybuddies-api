namespace API.Services.Implementations;

public class BlobStorageSettings
{
    public string ConnectionString { get; set; } = default!;
    public string ProfilePicturesContainer { get; set; } = default!;
    public string AccountKey { get; set; } = default!;
}
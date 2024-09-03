using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;
public class BlobService(ILogger<BlobService> logger, BlobContainerClient blobContainerClient)
{
    private readonly ILogger _logger = logger;
    private readonly BlobContainerClient _blobContainerClient = blobContainerClient;

    public async Task<Result<Stream?>> DownloadBlobAsync(string fileName, string dataHash)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            if (!await blobClient.ExistsAsync())
            {
                return Result.Success<Stream?>(null);
            }

            var blob = await blobClient.DownloadAsync();
            if (!blob.Value.Details.Metadata.TryGetValue(Constants.DataHashKey, out var hash) || hash != dataHash)
            {
                return Result.Success<Stream?>(null);
            }

            return Result.Success<Stream?>(blob.Value.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get the file");
            return Result.Error<Stream?>("Failed to get the file");
        }
    }

    public async Task<Result> UploadBlobAsync(string fileName, string dataHash, Stream stream)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [Constants.DataHashKey] = dataHash,
                },
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload the file");
            return Result.Error("Failed to upload the file");
        }

        return Result.Success();
    }
}

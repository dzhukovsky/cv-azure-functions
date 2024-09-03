using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;
public class FileService(ILogger<FileService> logger, ShareDirectoryClient shareDirectoryClient)
{
    private readonly ILogger _logger = logger;
    private readonly ShareDirectoryClient _shareDirectoryClient = shareDirectoryClient;

    public async Task<Result<Stream?>> DownloadAsync(string fileName, string dataHash)
    {
        try
        {
            var fileClient = _shareDirectoryClient.GetFileClient(fileName);
            if (!await fileClient.ExistsAsync())
            {
                return Result.Success<Stream?>(null);
            }

            var file = await fileClient.DownloadAsync();
            if (!file.Value.Details.Metadata.TryGetValue(Constants.DataHashKey, out var hash) || hash != dataHash)
            {
                return Result.Success<Stream?>(null);
            }

            return Result.Success<Stream?>(file.Value.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get the file");
            return Result.Error<Stream?>("Failed to get the file");
        }
    }

    public async Task<Result> UploadAsync(string fileName, string dataHash, Stream stream)
    {
        try
        {
            var fileClient = _shareDirectoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(stream.Length, metadata: new Dictionary<string, string>
            {
                [Constants.DataHashKey] = dataHash,
            });

            await fileClient.UploadAsync(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload the file");
            return Result.Error("Failed to upload the file");
        }

        return Result.Success();
    }
}

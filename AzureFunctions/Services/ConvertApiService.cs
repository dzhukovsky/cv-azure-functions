using ConvertApiDotNet;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services;
public class ConvertApiService(ILogger<ConvertApiService> logger, ConvertApi convertApi)
{
    private readonly ILogger _logger = logger;
    private readonly ConvertApi _convertApi = convertApi;

    public async Task<Result<Stream>> ConvertDocxToPdfAsync(Stream docxStream)
    {
        try
        {
            var response = await _convertApi.ConvertAsync("docx", "pdf", new ConvertApiFileParam(docxStream, ".docx"));
            if (response.Files.Length == 0)
            {
                return Result.Error<Stream>("Failed to convert the file");
            }

            var result = await response.Files[0].FileStreamAsync();
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert the file");
            return Result.Error<Stream>("Failed to convert the file");
        }
    }
}

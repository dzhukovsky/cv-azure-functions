using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Models;
public record struct GetPdfModel(string FileName, string DataHash)
{
    public static Result<GetPdfModel> Bind(HttpRequest request)
    {
        var fileName = request.Query.FirstOrDefault("fileName");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Error<GetPdfModel>("The 'fileName' query parameter is required");
        }
        else if (!fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Error<GetPdfModel>("The 'fileName' must end with '.docx'");
        }

        fileName = fileName[..fileName.LastIndexOf('.')];

        var dataHash = request.Query.FirstOrDefault("dataHash");
        if (string.IsNullOrWhiteSpace(dataHash))
        {
            return Result.Error<GetPdfModel>("The 'dataHash' query parameter is required");
        }

        return Result.Success(new GetPdfModel(fileName, dataHash));
    }
}

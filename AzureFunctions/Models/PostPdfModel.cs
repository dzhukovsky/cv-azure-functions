using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Models;
public record struct PostPdfModel(string FileName, string DataHash, Stream Stream)
{
    public static Result<PostPdfModel> Bind(HttpRequest request)
    {
        var fileName = request.Query.FirstOrDefault("fileName");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Error<PostPdfModel>("The 'fileName' query parameter is required");
        }
        else if (!fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Error<PostPdfModel>("The 'fileName' must end with '.docx'");
        }

        fileName = fileName[..fileName.LastIndexOf('.')];

        var dataHash = request.Query.FirstOrDefault("dataHash");
        if (string.IsNullOrWhiteSpace(dataHash))
        {
            return Result.Error<PostPdfModel>("The 'dataHash' query parameter is required");
        }

        if (!Constants.ContentTypeDocx.Equals(request.ContentType, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Error<PostPdfModel>($"Content-Type must be {Constants.ContentTypeDocx}");
        }


        return Result.Success(new PostPdfModel(fileName, dataHash, request.Body));
    }
}

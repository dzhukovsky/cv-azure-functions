using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Infrastructure.Validation;
public static class ResultExtensions
{
    public static BadRequestObjectResult ToBadRequest(this Result result) => new(result.Failure);
    public static BadRequestObjectResult ToBadRequest<T>(this Result<T> result) => new(result.Failure);

    public static ContentResult ToInternalServerError(this Result result) => new()
    {
        StatusCode = StatusCodes.Status500InternalServerError,
        Content = result.Failure
    };

    public static ContentResult ToInternalServerError<T>(this Result<T> result) => new()
    {
        StatusCode = StatusCodes.Status500InternalServerError,
        Content = result.Failure
    };

    public static FileStreamResult ToPdfResult(this Result<Stream> result, string fileName) => new(result.Value, "application/pdf")
    {
        FileDownloadName = fileName
    };
}

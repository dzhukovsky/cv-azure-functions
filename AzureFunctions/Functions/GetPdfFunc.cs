using AzureFunctions.Models;
using AzureFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Functions;
public class GetPdfFunc(FileService fileService)
{
    private readonly FileService _fileService = fileService;

    [Function("get-pdf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var modelResult = GetPdfModel.Bind(req);
        if (modelResult.IsFailure)
        {
            return modelResult.ToBadRequest();
        }

        var model = modelResult.Value;
        var pdfFileName = $"{model.FileName}.pdf";

        var fileResult = await _fileService.DownloadAsync(pdfFileName, model.DataHash);
        if (fileResult.IsFailure)
        {
            return fileResult.ToInternalServerError();
        }
        else if (fileResult.Value != null)
        {
            return fileResult!.ToPdfResult(pdfFileName);
        }

        return new NotFoundResult();
    }
}

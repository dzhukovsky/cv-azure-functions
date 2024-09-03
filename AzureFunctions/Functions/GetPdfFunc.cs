using AzureFunctions.Models;
using AzureFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Functions;
public class GetPdfFunc(ConvertApiService convertApiService, BlobService blobService)
{
    private readonly ConvertApiService _convertApiService = convertApiService;
    private readonly BlobService _blobService = blobService;

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

        var blobResult = await _blobService.DownloadBlobAsync(pdfFileName, model.DataHash);
        if (blobResult.IsFailure)
        {
            return blobResult.ToInternalServerError();
        }
        else if (blobResult.Value != null)
        {
            return blobResult!.ToPdfResult(pdfFileName);
        }

        return new NotFoundResult();
    }
}

using AzureFunctions.Models;
using AzureFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Functions;
public class PostPdfFunc(ConvertApiService convertApiService, BlobService blobService)
{
    private readonly ConvertApiService _convertApiService = convertApiService;
    private readonly BlobService _blobService = blobService;

    [Function("post-pdf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var modelResult = PostPdfModel.Bind(req);
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

        var pdfStreamResult = await _convertApiService.ConvertDocxToPdfAsync(model.Stream);
        if (pdfStreamResult.IsFailure)
        {
            return pdfStreamResult.ToInternalServerError();
        }

        var pdfStream = pdfStreamResult.Value;
        var uploadResult = await _blobService.UploadBlobAsync(pdfFileName, model.DataHash, pdfStream);
        if (uploadResult.IsFailure)
        {
            return uploadResult.ToInternalServerError();
        }

        pdfStream.Seek(0, SeekOrigin.Begin);
        return pdfStreamResult.ToPdfResult(pdfFileName);
    }
}

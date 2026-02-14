using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.UseCases.BulkNotesUseCases;
using UniversiteRestApi.Controllers.Models;

namespace UniversiteRestApi.Controllers;

[ApiController]
[Route("api/ues/{ueId:long}/notes/bulk")]
public class BulkNotesController : ControllerBase
{
    [HttpGet("template")]
    [Authorize]
    public async Task<IActionResult> Template(
        long ueId,
        [FromServices] GenerateUeNotesCsvTemplateUseCase useCase)
    {
        var bytes = await useCase.ExecuteAsync(ueId);
        return File(bytes, "text/csv", $"notes_ue_{ueId}.csv");
    }

    [HttpPost("import")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(
        long ueId,
        [FromForm] BulkNotesImportRequest request,
        [FromServices] ImportUeNotesCsvUseCase useCase)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("Fichier manquant.");

        await using var stream = request.File.OpenReadStream();
        var result = await useCase.ExecuteAsync(ueId, stream);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
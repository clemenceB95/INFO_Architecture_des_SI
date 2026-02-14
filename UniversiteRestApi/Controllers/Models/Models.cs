using Microsoft.AspNetCore.Mvc;

namespace UniversiteRestApi.Controllers.Models;

public class BulkNotesImportRequest
{
    [FromForm(Name = "file")]
    public IFormFile File { get; set; } = default!;
}
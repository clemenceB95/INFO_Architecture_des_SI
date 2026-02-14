namespace UniversiteDomain.Dtos;

public class BulkNotesImportResultDto
{
    public bool Success { get; set; }
    public int SavedCount { get; set; }
    public List<BulkNotesImportErrorDto> Errors { get; set; } = new();
}
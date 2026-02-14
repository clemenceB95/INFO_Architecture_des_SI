namespace UniversiteDomain.DataAdapters.BulkNotes;

public interface IBulkNotesWriter
{
    Task UpsertNoteAsync(long etudiantId, long ueId, float valeur);
}
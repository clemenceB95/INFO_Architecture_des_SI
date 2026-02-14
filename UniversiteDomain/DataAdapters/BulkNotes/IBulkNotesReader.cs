using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters.BulkNotes;

public interface IBulkNotesReader
{
    Task<Ue?> GetUeAsync(long ueId);
    Task<List<Etudiant>> GetEtudiantsFollowingUeAsync(long ueId);
    Task<Dictionary<string, float>> GetExistingNotesByUeAsync(long ueId); // NumEtud -> note
}
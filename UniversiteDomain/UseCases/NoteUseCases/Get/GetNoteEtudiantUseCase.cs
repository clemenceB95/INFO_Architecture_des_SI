using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNoteEtudiantUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role, IUniversiteUser user, long etudiantId)
    {
        if (role == Roles.Responsable || role == Roles.Scolarite) return true;
        if (role == Roles.Etudiant && user?.EtudiantId == etudiantId) return true;
        return false;
    }

    public async Task<List<Note>> ExecuteAsync(long etudiantId)
    {
        var notes = await factory.NoteRepository().GetByEtudiantIdWithUeAsync(etudiantId);

        return notes ?? new List<Note>();
    }
}
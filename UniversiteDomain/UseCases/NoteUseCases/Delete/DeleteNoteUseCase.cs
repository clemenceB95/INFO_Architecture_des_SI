using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Delete;

public class DeleteNoteUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task ExecuteAsync(long etudiantId, long ueId)
    {
        var note = await factory.NoteRepository().FindAsync(etudiantId, ueId);
        if (note == null) throw new Exception("Note introuvable");

        await factory.NoteRepository().DeleteAsync(note);

    }
}
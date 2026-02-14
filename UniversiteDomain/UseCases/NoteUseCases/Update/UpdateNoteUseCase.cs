using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        if (valeur < 0 || valeur > 20)
            throw new Exception("La note doit être comprise entre 0 et 20.");
        
        var note = await factory.NoteRepository().FindAsync(etudiantId, ueId);
        if (note == null)
            throw new Exception("Note introuvable");

        note.Valeur = valeur;
        
        await factory.SaveChangesAsync();
    }
}
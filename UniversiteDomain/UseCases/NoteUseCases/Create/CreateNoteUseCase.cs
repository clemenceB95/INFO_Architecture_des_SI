using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory factory)
{
    private readonly IRepositoryFactory _factory =
        factory ?? throw new ArgumentNullException(nameof(factory));

    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        if (etudiantId <= 0) throw new ArgumentOutOfRangeException(nameof(etudiantId));
        if (ueId <= 0) throw new ArgumentOutOfRangeException(nameof(ueId));
        
        if (valeur < 0 || valeur > 20)
            throw new ArgumentOutOfRangeException(nameof(valeur), "La note doit être comprise entre 0 et 20.");

        var repo = _factory.NoteRepository();
        
        var existantes = await repo.FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        if (existantes != null && existantes.Any())
            throw new InvalidOperationException($"Une note existe déjà pour EtudiantId={etudiantId} et UeId={ueId}.");

        var note = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = valeur
        };

        var result = await repo.CreateAsync(note);
        await _factory.SaveChangesAsync();
        return result;
    }
    
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;
}
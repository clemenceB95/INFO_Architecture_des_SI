using System;
using System.Linq;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Add;

public class AddNoteEtudiantDansUeUseCase(IRepositoryFactory factory)
{
    private readonly IRepositoryFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    public async Task<Note> ExecuteAsync(long idEtudiant, long idUe, float valeur)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);

        // Règle : note entre 0 et 20
        if (valeur < 0 || valeur > 20)
            throw new InvalidNoteValueException(valeur);

        var etuRepo = _factory.EtudiantRepository();
        var ueRepo = _factory.UeRepository();
        var noteRepo = _factory.NoteRepository();

        // Vérifier étudiant
        var etudiants = await etuRepo.FindByConditionAsync(e => e.Id == idEtudiant);
        if (etudiants == null || etudiants.Count == 0)
            throw new EtudiantNotFoundException(idEtudiant.ToString());

        var etudiant = etudiants[0];

        // Vérifier UE
        var ues = await ueRepo.FindByConditionAsync(u => u.Id == idUe);
        if (ues == null || ues.Count == 0)
            throw new UeNotFoundException(idUe.ToString());

        // Vérifier parcours de l'étudiant
        if (etudiant.ParcoursSuivi == null)
            throw new ParcoursNotFoundException($"Aucun parcours associé à l'étudiant {idEtudiant}");

        // Règle : UE doit être dans le parcours de l'étudiant
        if (etudiant.ParcoursSuivi.UesEnseignees == null ||
            !etudiant.ParcoursSuivi.UesEnseignees.Any(u => u.Id == idUe))
            throw new UeNotInParcoursException(idEtudiant, idUe);

        // Règle : 1 note max par (Etudiant, UE)
        var deja = await noteRepo.FindByConditionAsync(n => n.EtudiantId == idEtudiant && n.UeId == idUe);
        if (deja != null && deja.Any())
            throw new DuplicateNoteException(idEtudiant, idUe);

        // Création
        var note = new Note { EtudiantId = idEtudiant, UeId = idUe, Valeur = valeur };
        var created = await noteRepo.CreateAsync(note);

        await _factory.SaveChangesAsync();
        return created;
    }
}

using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        await factory.EtudiantRepository().UpdateAsync(etudiant);
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(etudiant);

        IEtudiantRepository etudiantRepository = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);

        if (etudiant.Id <= 0) throw new ArgumentException("Id étudiant invalide.");
        if (string.IsNullOrWhiteSpace(etudiant.NumEtud)) throw new ArgumentException("NumEtud obligatoire.");
        if (string.IsNullOrWhiteSpace(etudiant.Nom)) throw new ArgumentException("Nom obligatoire.");
        if (string.IsNullOrWhiteSpace(etudiant.Prenom)) throw new ArgumentException("Prénom obligatoire.");
        if (string.IsNullOrWhiteSpace(etudiant.Email)) throw new ArgumentException("Email obligatoire.");

        await Task.CompletedTask;
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}
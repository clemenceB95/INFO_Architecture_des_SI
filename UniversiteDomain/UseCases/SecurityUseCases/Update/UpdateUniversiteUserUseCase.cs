using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);

        IUniversiteUserRepository userRepo = factory.UniversiteUserRepository();

        // On retrouve le user avec l'email actuel
        IUniversiteUser user = await userRepo.FindByEmailAsync(etudiant.Email);
        if (user == null) throw new InvalidOperationException("Utilisateur introuvable.");

        // Mise à jour du user (UserName et Email)
        await userRepo.UpdateAsync(user, etudiant.Email, etudiant.Email);
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(etudiant);

        IUniversiteUserRepository userRepo = factory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(userRepo);

        if (etudiant.Id <= 0) throw new ArgumentException("Id étudiant invalide.");
        if (string.IsNullOrWhiteSpace(etudiant.Email)) throw new ArgumentException("Email obligatoire.");

        await Task.CompletedTask;
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}
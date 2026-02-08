using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules(idEtudiant);
        
        IEtudiantRepository etuRepo = factory.EtudiantRepository();
        Etudiant? etudiant = await etuRepo.FindAsync(idEtudiant);
        if (etudiant == null) return;
        
        IUniversiteUserRepository userRepo = factory.UniversiteUserRepository();
        IUniversiteUser user = await userRepo.FindByEmailAsync(etudiant.Email);
        if (user == null) return;

        await userRepo.DeleteAsync(user);
    }

    private async Task CheckBusinessRules(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);

        IUniversiteUserRepository userRepo = factory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(userRepo);

        IEtudiantRepository etudiantRepo = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepo);

        if (idEtudiant <= 0) throw new ArgumentException("Id étudiant invalide.");

        await Task.CompletedTask;
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}
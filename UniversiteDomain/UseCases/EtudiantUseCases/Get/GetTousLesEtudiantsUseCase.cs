using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant>> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Etudiant> etudiants = await factory.EtudiantRepository().FindAllAsync();
        return etudiants;
    }

    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        await Task.CompletedTask;
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}
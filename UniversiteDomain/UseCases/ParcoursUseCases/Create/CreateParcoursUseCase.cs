using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase
{
    private readonly IRepositoryFactory factory;

    public CreateParcoursUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);

        var repo = factory.ParcoursRepository();

        // Vérifie si le parcours existe déjà
        var existants = await repo.FindByConditionAsync(p => p.Id == parcours.Id);
        if (existants != null && existants.Count > 0)
            throw new Exception("Le parcours existe déjà");

        // Création
        var created = await repo.CreateAsync(parcours);

        await factory.SaveChangesAsync();
        return created;
    }
}
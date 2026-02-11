using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase
{
    private readonly IRepositoryFactory factory;

    public UpdateParcoursUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task ExecuteAsync(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);

        var repo = factory.ParcoursRepository();
        
        var existants = await repo.FindByConditionAsync(p => p.Id == parcours.Id);
        if (existants is not { Count: > 0 })
            throw new Exception("Parcours introuvable");

        await CheckBusinessRules(parcours);
        
        var existant = existants[0];
        existant.NomParcours = parcours.NomParcours;
        existant.AnneeFormation = parcours.AnneeFormation;
        
        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        if (string.IsNullOrWhiteSpace(parcours.NomParcours))
            throw new Exception("Nom du parcours obligatoire");

        // Unicité du nom
        var repo = factory.ParcoursRepository();
        var existe = await repo.FindByConditionAsync(p =>
            p.NomParcours == parcours.NomParcours && p.Id != parcours.Id);

        if (existe is { Count: > 0 })
            throw new Exception("Un parcours avec ce nom existe déjà");
    }
}
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase
{
    private readonly IRepositoryFactory factory;

    public UpdateUeUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);

        var repo = factory.UeRepository();
        var existants = await repo.FindByConditionAsync(x => x.Id == ue.Id);
        if (existants is not { Count: > 0 })
            throw new Exception("UE introuvable");

        await CheckBusinessRules(ue);
        
        var existante = existants[0];
        existante.NumeroUe = ue.NumeroUe;
        existante.Intitule = ue.Intitule;

        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        if (string.IsNullOrWhiteSpace(ue.NumeroUe))
            throw new Exception("Numéro UE obligatoire");
        if (string.IsNullOrWhiteSpace(ue.Intitule))
            throw new Exception("Intitulé UE obligatoire");

        var repo = factory.UeRepository();

        var existe = await repo.FindByConditionAsync(u => u.NumeroUe == ue.NumeroUe && u.Id != ue.Id);
        if (existe is { Count: > 0 })
            throw new Exception("Une UE avec ce numéro existe déjà");
    }
}
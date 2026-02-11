using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetParcoursByIdUseCase
{
    private readonly IRepositoryFactory factory;

    public GetParcoursByIdUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task<Parcours?> ExecuteAsync(long id)
    {
        var repo = factory.ParcoursRepository();
        var res = await repo.FindByConditionAsync(p => p.Id == id);
        return res.FirstOrDefault();
    }
}
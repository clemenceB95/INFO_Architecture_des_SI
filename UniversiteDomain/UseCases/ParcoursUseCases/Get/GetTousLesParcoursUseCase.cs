using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetTousLesParcoursUseCase
{
    private readonly IRepositoryFactory _factory;

    public GetTousLesParcoursUseCase(IRepositoryFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task<List<Parcours>> ExecuteAsync()
    {
        var repo = _factory.ParcoursRepository();
        return await repo.FindByConditionAsync(_ => true);
    }
}
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetTousLesUesUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task<List<Ue>> ExecuteAsync()
    {
        var ues = await factory.UeRepository().FindByConditionAsync(_ => true);
        return ues ?? new List<Ue>();
    }
}
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeByIdUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task<Ue?> ExecuteAsync(long id)
    {
        var res = await factory.UeRepository().FindByConditionAsync(u => u.Id == id);
        return res.FirstOrDefault();
    }
}
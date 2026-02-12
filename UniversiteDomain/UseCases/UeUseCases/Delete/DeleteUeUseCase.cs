using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;

public class DeleteUeUseCase
{
    private readonly IRepositoryFactory factory;

    public DeleteUeUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;

    public async Task ExecuteAsync(long id)
    {
        var repo = factory.UeRepository();

        var existants = await repo.FindByConditionAsync(u => u.Id == id);
        if (existants is not { Count: > 0 })
            throw new Exception("UE introuvable");

        await repo.DeleteAsync(id);
        await factory.SaveChangesAsync();
    }
}
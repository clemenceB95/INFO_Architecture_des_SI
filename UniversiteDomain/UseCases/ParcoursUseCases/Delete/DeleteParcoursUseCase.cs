using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase
{
    private readonly IRepositoryFactory factory;

    public DeleteParcoursUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task ExecuteAsync(long id)
    {
        var repo = factory.ParcoursRepository();

        // Vérifie que le parcours existe
        var existant = await repo.FindByConditionAsync(p => p.Id == id);
        if (existant is not { Count: > 0 })
            throw new Exception("Parcours introuvable");

        await repo.DeleteAsync(id);
        await factory.SaveChangesAsync();
    }
}
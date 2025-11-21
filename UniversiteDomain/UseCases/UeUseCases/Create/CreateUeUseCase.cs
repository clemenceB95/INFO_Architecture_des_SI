using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory factory)
{
    private readonly IRepositoryFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);

        // Vérification du nom
        if (string.IsNullOrWhiteSpace(ue.NomUe) || ue.NomUe.Length <= 3)
            throw new InvalidUeNameException();

        var repo = _factory.UeRepository();

        // Vérification de l’unicité du code
        var existantes = await repo.FindByConditionAsync(u => u.Code == ue.Code);
        if (existantes.Any())
            throw new DuplicateCodeUeException(ue.Code);
        
        // Création si tout est bon
        var result = await repo.CreateAsync(ue);

        // Sauvegarde des modifications
        await _factory.SaveChangesAsync();

        return result;
    }
}
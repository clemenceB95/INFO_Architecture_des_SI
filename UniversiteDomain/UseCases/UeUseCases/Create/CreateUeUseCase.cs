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

        if (string.IsNullOrWhiteSpace(ue.Code))
            throw new ArgumentException("Le code de l'UE est obligatoire.", nameof(ue));

        // Vérification du nom (>3 caractères)
        if (string.IsNullOrWhiteSpace(ue.NomUe) || ue.NomUe.Trim().Length <= 3)
            throw new InvalidUeNameException();

        var repo = _factory.UeRepository();

        // Vérification de l’unicité du code
        var existantes = await repo.FindByConditionAsync(u => u.Code == ue.Code);
        if (existantes != null && existantes.Any())
            throw new DuplicateCodeUeException(ue.Code);

        var result = await repo.CreateAsync(ue);
        await _factory.SaveChangesAsync();
        return result;
    }
}
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

        if (string.IsNullOrWhiteSpace(ue.NumeroUe))
            throw new ArgumentException("Le numéro de l'UE est obligatoire.", nameof(ue));

        // Règle : intitulé > 3 caractères
        if (string.IsNullOrWhiteSpace(ue.Intitule) || ue.Intitule.Trim().Length <= 3)
            throw new InvalidUeNameException();

        var repo = _factory.UeRepository();

        // Règle : pas 2 UE avec le même numéro
        var existantes = await repo.FindByConditionAsync(u => u.NumeroUe == ue.NumeroUe);
        if (existantes != null && existantes.Any())
            throw new DuplicateCodeUeException(ue.NumeroUe);

        var result = await repo.CreateAsync(ue);
        await _factory.SaveChangesAsync();
        return result;
    }
    
    public bool IsAuthorized(string role)
        => role == Roles.Responsable || role == Roles.Scolarite;
}
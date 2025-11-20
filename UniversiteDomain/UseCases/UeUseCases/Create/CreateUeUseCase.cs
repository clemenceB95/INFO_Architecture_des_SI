using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase
{
    private readonly IUeRepository _ueRepository;

    public CreateUeUseCase(IUeRepository ueRepository)
    {
        _ueRepository = ueRepository 
                        ?? throw new ArgumentNullException(nameof(ueRepository));
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);

        // Vérification du nom
        if (string.IsNullOrWhiteSpace(ue.NomUe) || ue.NomUe.Length <= 3)
            throw new InvalidUeNameException();

        // Vérification de l’unicité du code
        var existantes = await _ueRepository.FindByConditionAsync(u => u.Code == ue.Code);
        if (existantes != null && existantes.Count > 0)
            throw new DuplicateCodeUeException(ue.Code);

        // Création si tout est bon
        return await _ueRepository.CreateAsync(ue);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Ajout d'une UE via objets
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ue);
        return await ExecuteAsync(parcours.Id, ue.Id);
    }

    // Ajout d'une UE via ids
    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
    {
        await CheckBusinessRules(idParcours, idUe);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
    }

    // Ajout de plusieurs UE via objets
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ues);

        long[] idUes = ues.Select(x => x.Id).ToArray();
        return await ExecuteAsync(parcours.Id, idUes);
    }

    // Ajout de plusieurs UE via ids
    public async Task<Parcours> ExecuteAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(idUes);

        // Comme demandé : vérifier toutes les règles avant modification
        foreach (var idUe in idUes)
            await CheckBusinessRules(idParcours, idUe);

        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUes);
    }

    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        // Vérification des paramètres
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);

        // Vérifions que les datasources existent
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());

        // On recherche l'UE
        List<Ue>? ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.Id == idUe);

        if (ues == null || ues.Count == 0)
            throw new UeNotFoundException(idUe.ToString());

        // On recherche le parcours
        List<Parcours>? parcoursList = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.Id == idParcours);

        if (parcoursList == null || parcoursList.Count == 0)
            throw new ParcoursNotFoundException(idParcours.ToString());

        var parcours = parcoursList[0];

        // Règle : ne pas dupliquer une UE dans un parcours
        if (parcours.UesEnseignees != null && parcours.UesEnseignees.Any(u => u.Id == idUe))
            throw new DuplicateUeDansParcoursException(
                $"{idUe} est déjà présente dans le parcours : {idParcours}"
            );
    }
}

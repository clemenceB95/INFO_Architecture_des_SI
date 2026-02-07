using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context)
    : Repository<Parcours>(context), IParcoursRepository
{

    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);

        parcours.Inscrits ??= new List<Etudiant>();
        if (!parcours.Inscrits.Any(e => e.Id == etudiant.Id))
            parcours.Inscrits.Add(etudiant);

        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;

        return await AddEtudiantAsync(p, e);
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiants);

        foreach (var e in etudiants)
        {
            await AddEtudiantAsync(parcours, e);
        }

        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(idEtudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;

        foreach (var idEtudiant in idEtudiants)
        {
            Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
            await AddEtudiantAsync(p, e);
        }

        return p;
    }
    

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ue);

        parcours.UesEnseignees ??= new List<Ue>();
        if (!parcours.UesEnseignees.Any(x => x.Id == ue.Id))
            parcours.UesEnseignees.Add(ue);

        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        Ue ue = (await Context.Ues.FindAsync(idUe))!;

        return await AddUeAsync(p, ue);
    }

    public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ues);

        foreach (var ue in ues)
        {
            await AddUeAsync(parcours, ue);
        }

        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(idUes);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;

        foreach (var idUe in idUes)
        {
            Ue ue = (await Context.Ues.FindAsync(idUe))!;
            await AddUeAsync(p, ue);
        }

        return p;
    }
}

using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters.BulkNotes;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories.BulkNotes;

public class BulkNotesRepository(UniversiteDbContext context) : IBulkNotesReader, IBulkNotesWriter
{
    public Task<Ue?> GetUeAsync(long ueId)
        => context.Ues
            .Include(u => u.EnseigneeDans)
            .FirstOrDefaultAsync(u => u.Id == ueId);

    public async Task<List<Etudiant>> GetEtudiantsFollowingUeAsync(long ueId)
    {
        var parcoursIds = await context.Ues
            .Where(u => u.Id == ueId)
            .SelectMany(u => u.EnseigneeDans.Select(p => p.Id))
            .ToListAsync();

        return await context.Etudiants
            .Include(e => e.ParcoursSuivi)
            .Where(e => e.ParcoursSuivi != null && parcoursIds.Contains(e.ParcoursSuivi.Id))
            .OrderBy(e => e.NumEtud)
            .ToListAsync();
    }

    public async Task<Dictionary<string, float>> GetExistingNotesByUeAsync(long ueId)
    {
        return await context.Notes
            .AsNoTracking()
            .Include(n => n.Etudiant)
            .Where(n => n.UeId == ueId && n.Etudiant != null)
            .ToDictionaryAsync(n => n.Etudiant!.NumEtud, n => n.Valeur);
    }

    public async Task UpsertNoteAsync(long etudiantId, long ueId, float valeur)
    {
        var existing = await context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

        if (existing == null)
        {
            await context.Notes.AddAsync(new Note
            {
                EtudiantId = etudiantId,
                UeId = ueId,
                Valeur = valeur
            });
        }
        else
        {
            existing.Valeur = valeur;
        }
    }
}
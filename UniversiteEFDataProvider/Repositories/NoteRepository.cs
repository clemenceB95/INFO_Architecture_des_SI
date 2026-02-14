using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context)
    : Repository<Note>(context), INoteRepository
{
    public async Task<List<Note>> GetByEtudiantIdWithUeAsync(long etudiantId)
    {
        return await context.Set<Note>()
            .AsNoTracking()
            .Include(n => n.Ue)
            .Where(n => n.EtudiantId == etudiantId)
            .ToListAsync();
    }
}
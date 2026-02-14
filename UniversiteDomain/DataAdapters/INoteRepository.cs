using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task<List<Note>> GetByEtudiantIdWithUeAsync(long etudiantId);
}
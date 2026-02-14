using Microsoft.EntityFrameworkCore.Storage;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class EfUnitOfWork(UniversiteDbContext db) : IUnitOfWork
{
    private IDbContextTransaction? _tx;

    public async Task BeginTransactionAsync()
    {
        _tx = await db.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await db.SaveChangesAsync();

        if (_tx != null)
            await _tx.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_tx != null)
            await _tx.RollbackAsync();
    }
}
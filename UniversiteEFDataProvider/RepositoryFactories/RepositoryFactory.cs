using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(UniversiteDbContext context) : IRepositoryFactory
{
    private readonly UniversiteDbContext _context =
        context ?? throw new InvalidOperationException();

    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;

    public IParcoursRepository ParcoursRepository()
        => _parcours ??= new ParcoursRepository(_context);

    public IEtudiantRepository EtudiantRepository()
        => _etudiants ??= new EtudiantRepository(_context);

    public IUeRepository UeRepository()
        => _ues ??= new UeRepository(_context);

    public INoteRepository NoteRepository()
        => _notes ??= new NoteRepository(_context);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();

    public Task EnsureCreatedAsync()
        => _context.Database.EnsureCreatedAsync();

    public Task EnsureDeletedAsync()
        => _context.Database.EnsureDeletedAsync();
}
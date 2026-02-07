using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(
    UniversiteDbContext context,
    RoleManager<UniversiteRole> roleManager,
    UserManager<UniversiteUser> userManager
) : IRepositoryFactory
{
    private readonly UniversiteDbContext _context =
        context ?? throw new InvalidOperationException();

    private readonly RoleManager<UniversiteRole> _roleManager =
        roleManager ?? throw new InvalidOperationException();

    private readonly UserManager<UniversiteUser> _userManager =
        userManager ?? throw new InvalidOperationException();

    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;

    private IUniversiteRoleRepository? _universiteRoles;
    private IUniversiteUserRepository? _universiteUsers;

    public IParcoursRepository ParcoursRepository()
        => _parcours ??= new ParcoursRepository(_context);

    public IEtudiantRepository EtudiantRepository()
        => _etudiants ??= new EtudiantRepository(_context);

    public IUeRepository UeRepository()
        => _ues ??= new UeRepository(_context);

    public INoteRepository NoteRepository()
        => _notes ??= new NoteRepository(_context);

    public IUniversiteRoleRepository UniversiteRoleRepository()
        => _universiteRoles ??= new UniversiteRoleRepository(_context, _roleManager);

    public IUniversiteUserRepository UniversiteUserRepository()
        => _universiteUsers ??= new UniversiteUserRepository(_context, _userManager, _roleManager);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();

    public Task EnsureCreatedAsync()
        => _context.Database.EnsureCreatedAsync();

    public Task EnsureDeletedAsync()
        => _context.Database.EnsureDeletedAsync();
}

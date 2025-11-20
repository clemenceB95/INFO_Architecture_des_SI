using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

public class CreateEtudiantUseCase
{
    private readonly IRepositoryFactory factory;

    public CreateEtudiantUseCase(IRepositoryFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        return await ExecuteAsync(etudiant);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);

        await CheckBusinessRules(etudiant);

        // On récupère le repo depuis la factory
        var repo = factory.EtudiantRepository();

        Etudiant et = await repo.CreateAsync(etudiant);
        await factory.SaveChangesAsync();
        return et;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        var repo = factory.EtudiantRepository();

        // Unicité du numéro
        var existe = await repo.FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
        if (existe is { Count: > 0 })
            throw new DuplicateNumEtudException($"{etudiant.NumEtud} - ce numéro d'étudiant est déjà affecté à un étudiant");

        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException($"{etudiant.Email} - Email mal formé");

        // Unicité de l'email
        existe = await repo.FindByConditionAsync(e => e.Email.Equals(etudiant.Email));
        if (existe is { Count: > 0 })
            throw new DuplicateEmailException($"{etudiant.Email} est déjà affecté à un étudiant");

        // Nom > 3 caractères
        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException($"{etudiant.Nom} incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
}

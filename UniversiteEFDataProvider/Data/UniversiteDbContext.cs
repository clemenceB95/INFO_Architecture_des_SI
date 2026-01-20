using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Data;

public class UniversiteDbContext : DbContext
{
    // Logger pour afficher les requêtes SQL dans la console
    private static readonly ILoggerFactory ConsoleLogger =
        LoggerFactory.Create(builder => builder.AddConsole());

    // Constructeur avec injection de dépendances
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }

    // Constructeur par défaut
    public UniversiteDbContext()
    {
    }

    // Configuration générale du contexte (logging, erreurs détaillées)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(ConsoleLogger)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    // Configuration du mapping objet / relationnel
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // Etudiant
        // =========================
        modelBuilder.Entity<Etudiant>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.ParcoursSuivi)
            .WithMany(p => p.Inscrits);

        modelBuilder.Entity<Etudiant>()
            .HasMany(e => e.NotesObtenues)
            .WithOne(n => n.Etudiant);

        // =========================
        // Parcours
        // =========================
        modelBuilder.Entity<Parcours>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.UesEnseignees)
            .WithMany(ue => ue.EnseigneeDans);

        // =========================
        // UE
        // =========================
        modelBuilder.Entity<Ue>()
            .HasKey(ue => ue.Id);

        modelBuilder.Entity<Ue>()
            .HasMany(ue => ue.Notes)
            .WithOne(n => n.Ue);

        // =========================
        // Note (clé primaire composite)
        // =========================
        modelBuilder.Entity<Note>()
            .HasKey(n => new { n.EtudiantId, n.UeId });

        modelBuilder.Entity<Note>()
            .HasOne(n => n.Etudiant)
            .WithMany(e => e.NotesObtenues);

        modelBuilder.Entity<Note>()
            .HasOne(n => n.Ue)
            .WithMany(ue => ue.Notes);
    }
    public DbSet<Parcours>? Parcours { get; set; }
    public DbSet<Etudiant>? Etudiants { get; set; }
    public DbSet<Ue>? Ues { get; set; }
    public DbSet<Note>? Notes { get; set; }

}

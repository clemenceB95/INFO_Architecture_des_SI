using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Add;

namespace UniversiteDomainUnitTest;

public class NoteUnitTest
{
    [Test]
    public async Task AddNoteEtudiantDansUe_ShouldCreate_WhenValid()
    {
        long idEtudiant = 1;
        long idUe = 10;
        float valeur = 15.5f;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };
        var parcours = new Parcours
        {
            Id = 3,
            NomParcours = "Master",
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };

        var etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1",
            ParcoursSuivi = parcours
        };

        // Note sans Id (clé composite EtudiantId + UeId)
        var noteCreee = new Note { EtudiantId = idEtudiant, UeId = idUe, Valeur = valeur };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockNoteRepo = new Mock<INoteRepository>();
        mockNoteRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>()); // pas de note existante

        mockNoteRepo
            .Setup(r => r.CreateAsync(It.IsAny<Note>()))
            .ReturnsAsync(noteCreee);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);
        mockFactory.Setup(f => f.SaveChangesAsync()).Returns(Task.CompletedTask);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        var result = await useCase.ExecuteAsync(idEtudiant, idUe, valeur);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.EtudiantId, Is.EqualTo(idEtudiant));
        Assert.That(result.UeId, Is.EqualTo(idUe));
        Assert.That(result.Valeur, Is.EqualTo(valeur));

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Once);
        mockFactory.Verify(f => f.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_InvalidNoteValue_WhenOutOfRange()
    {
        var mockFactory = new Mock<IRepositoryFactory>();
        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidNoteValueException>(async () =>
            await useCase.ExecuteAsync(1, 10, 25f)
        );
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_EtudiantNotFound_WhenMissing()
    {
        long idEtudiant = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>()); // étudiant non trouvé

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockNoteRepo = new Mock<INoteRepository>();

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<EtudiantNotFoundException>(async () =>
            await useCase.ExecuteAsync(idEtudiant, idUe, 12f)
        );

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_UeNotFound_WhenMissing()
    {
        long idEtudiant = 1;
        long idUe = 10;

        var parcours = new Parcours
        {
            Id = 3,
            NomParcours = "Master",
            AnneeFormation = 1,
            UesEnseignees = new List<Ue>()
        };

        var etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1",
            ParcoursSuivi = parcours
        };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>()); // UE non trouvée

        var mockNoteRepo = new Mock<INoteRepository>();

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<UeNotFoundException>(async () =>
            await useCase.ExecuteAsync(idEtudiant, idUe, 12f)
        );

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_ParcoursNotFound_WhenNoParcours()
    {
        long idEtudiant = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };

        var etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1",
            ParcoursSuivi = null
        };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockNoteRepo = new Mock<INoteRepository>();

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<ParcoursNotFoundException>(async () =>
            await useCase.ExecuteAsync(idEtudiant, idUe, 12f)
        );

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_UeNotInParcours_WhenUeNotInStudentParcours()
    {
        long idEtudiant = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };

        // Parcours sans l'UE
        var parcours = new Parcours
        {
            Id = 3,
            NomParcours = "Master",
            AnneeFormation = 1,
            UesEnseignees = new List<Ue>()
        };

        var etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1",
            ParcoursSuivi = parcours
        };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockNoteRepo = new Mock<INoteRepository>();
        mockNoteRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<UeNotInParcoursException>(async () =>
            await useCase.ExecuteAsync(idEtudiant, idUe, 12f)
        );

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Test]
    public void AddNoteEtudiantDansUe_ShouldThrow_DuplicateNote_WhenAlreadyExists()
    {
        long idEtudiant = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };
        var parcours = new Parcours
        {
            Id = 3,
            NomParcours = "Master",
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };

        var etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "1",
            ParcoursSuivi = parcours
        };

        // Note existante sans Id
        var noteExistante = new Note { EtudiantId = idEtudiant, UeId = idUe, Valeur = 14f };

        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockNoteRepo = new Mock<INoteRepository>();
        mockNoteRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note> { noteExistante });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

        var useCase = new AddNoteEtudiantDansUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<DuplicateNoteException>(async () =>
            await useCase.ExecuteAsync(idEtudiant, idUe, 12f)
        );

        mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }
}

using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTest;

public class AddUeDansParcoursUnitTest
{
    [Test]
    public async Task AddUeDansParcours_ShouldAdd_WhenValid()
    {
        long idParcours = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };
        var parcoursAvant = new Parcours { Id = idParcours, NomParcours = "Master", AnneeFormation = 1, UesEnseignees = new List<Ue>() };

        var parcoursApres = new Parcours { Id = idParcours, NomParcours = "Master", AnneeFormation = 1, UesEnseignees = new List<Ue> { ue } };

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockParcoursRepo = new Mock<IParcoursRepository>();
        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcoursAvant });

        mockParcoursRepo
            .Setup(r => r.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursApres);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        var result = await useCase.ExecuteAsync(idParcours, idUe);

        Assert.That(result.UesEnseignees, Is.Not.Null);
        Assert.That(result.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(result.UesEnseignees[0].Id, Is.EqualTo(idUe));

        mockParcoursRepo.Verify(r => r.AddUeAsync(idParcours, idUe), Times.Once);
    }

    [Test]
    public void AddUeDansParcours_ShouldThrow_UeNotFound_WhenUeMissing()
    {
        long idParcours = 1;
        long idUe = 10;

        var parcours = new Parcours { Id = idParcours, NomParcours = "Master", AnneeFormation = 1, UesEnseignees = new List<Ue>() };

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>()); // UE non trouvée

        var mockParcoursRepo = new Mock<IParcoursRepository>();
        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        Assert.ThrowsAsync<UeNotFoundException>(async () => await useCase.ExecuteAsync(idParcours, idUe));

        mockParcoursRepo.Verify(r => r.AddUeAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }

    [Test]
    public void AddUeDansParcours_ShouldThrow_Duplicate_WhenAlreadyInParcours()
    {
        long idParcours = 1;
        long idUe = 10;

        var ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Algorithmique" };

        // parcours contient déjà l'UE
        var parcours = new Parcours
        {
            Id = idParcours,
            NomParcours = "Master",
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };

        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        var mockParcoursRepo = new Mock<IParcoursRepository>();
        mockParcoursRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        Assert.ThrowsAsync<DuplicateUeDansParcoursException>(async () => await useCase.ExecuteAsync(idParcours, idUe));

        mockParcoursRepo.Verify(r => r.AddUeAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }
}

using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTest
{
    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        string nomParcours = "Ue 1";
        int anneeFormation = 2;

        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursAvant = new Parcours { Id = idParcours, NomParcours = nomParcours, AnneeFormation = anneeFormation };

        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockParcours = new Mock<IParcoursRepository>();

        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que le parcours n'existe pas déjà
        mockParcours
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours>());

        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Parcours parcoursFinal = new Parcours { Id = idParcours, NomParcours = nomParcours, AnneeFormation = anneeFormation };
        mockParcours.Setup(repo => repo.CreateAsync(parcoursAvant)).ReturnsAsync(parcoursFinal);

        // On crée une fausse factory qui renvoie notre faux repository
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);

        // Le use case appelle SaveChangesAsync, donc on le simule aussi
        mockFactory.Setup(facto => facto.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Création du use case en utilisant le mockFactory comme datasource
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(mockFactory.Object);

        // Appel du use case
        var parcoursTeste = await useCase.ExecuteAsync(parcoursAvant);

        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));

        // Vérifie les appels
        mockFactory.Verify(f => f.ParcoursRepository(), Times.Once);
        mockParcours.Verify(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Parcours, bool>>>()), Times.Once);
        mockParcours.Verify(r => r.CreateAsync(parcoursAvant), Times.Once);
        mockFactory.Verify(f => f.SaveChangesAsync(), Times.Once);
    }
}


using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTest;

public class UeUnitTest
{
    [Test]
    public async Task CreateUeUseCase_ShouldCreate_WhenValid()
    {
        var ueAvant = new Ue
        {
            NumeroUe = "UE101",
            Intitule = "Algorithmique"
        };

        var ueFinal = new Ue
        {
            Id = 1,
            NumeroUe = "UE101",
            Intitule = "Algorithmique"
        };

        var mockRepo = new Mock<IUeRepository>();
        mockRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        mockRepo
            .Setup(r => r.CreateAsync(ueAvant))
            .ReturnsAsync(ueFinal);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockRepo.Object);
        mockFactory.Setup(f => f.SaveChangesAsync()).Returns(Task.CompletedTask);

        var useCase = new CreateUeUseCase(mockFactory.Object);

        var result = await useCase.ExecuteAsync(ueAvant);

        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.NumeroUe, Is.EqualTo("UE101"));
        Assert.That(result.Intitule, Is.EqualTo("Algorithmique"));
    }
}
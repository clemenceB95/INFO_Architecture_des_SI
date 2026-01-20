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
            Code = "MATH101",
            NomUe = "Algorithmique"
        };

        var ueFinal = new Ue
        {
            Id = 1,
            Code = "MATH101",
            NomUe = "Algorithmique"
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
        Assert.That(result.Code, Is.EqualTo("MATH101"));
        Assert.That(result.NomUe, Is.EqualTo("Algorithmique"));
    }
}
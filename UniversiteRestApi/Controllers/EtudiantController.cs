using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    // GET: api/Etudiant
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/Etudiant/5
    // Placeholder pour l'instant (nécessaire pour CreatedAtAction)
    [HttpGet("{id:long}")]
    public ActionResult<EtudiantDto> GetUnEtudiant(long id)
    {
        return NotFound();
    }

    // POST api/Etudiant
    [HttpPost]
    public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
    {
        CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory);
        Etudiant etud = etudiantDto.ToEntity();

        try
        {
            etud = await createEtudiantUc.ExecuteAsync(etud);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }

        EtudiantDto dto = new EtudiantDto().ToDto(etud);

        return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
    }

    // PUT api/Etudiant/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/Etudiant/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}





using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/<ParcoursController>
        [HttpGet]
        public async Task<ActionResult<List<ParcoursDto>>> GetAsync()
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            List<Parcours> parcours;
            try
            {
                parcours = await uc.ExecuteAsync();
            }
            catch (Exception e)
            {
                return ValidationProblem();
            }

            return ParcoursDto.ToDtos(parcours);
        }

        // GET api/<ParcoursController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>> GetUnParcours(long id)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            Parcours? parcours;
            try
            {
                parcours = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            if (parcours == null) return NotFound();

            return new ParcoursDto().ToDto(parcours);
        }

// POST api/<ParcoursController>
        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
        {
            CreateParcoursUseCase uc = new CreateParcoursUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            Parcours parcours = parcoursDto.ToEntity();

            try
            {
                parcours = await uc.ExecuteAsync(parcours);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            ParcoursDto dto = new ParcoursDto().ToDto(parcours);
            
            return CreatedAtAction(nameof(GetUnParcours), new { id = dto.Id }, dto);
        }

        // PUT api/<ParcoursController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ParcoursDto>> PutAsync(long id, [FromBody] ParcoursDto parcoursDto)
        {
            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);

            if (id != parcoursDto.Id)
            {
                return BadRequest();
            }

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(parcoursDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        // DELETE api/<ParcoursController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Parcours>> DeleteAsync(long id)
        {
            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        // POST api/Parcours/{parcoursId}/etudiants/{etudiantId}
        [HttpPost("{parcoursId}/etudiants/{etudiantId}")]
        public async Task<ActionResult> AddEtudiantDansParcoursAsync(long parcoursId, long etudiantId)
        {
            AddEtudiantDansParcoursUseCase uc = new AddEtudiantDansParcoursUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(parcoursId, etudiantId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        // POST api/Parcours/{parcoursId}/ues/{ueId}
        [HttpPost("{parcoursId}/ues/{ueId}")]
        public async Task<ActionResult> AddUeDansParcoursAsync(long parcoursId, long ueId)
        {
            AddUeDansParcoursUseCase uc = new AddUeDansParcoursUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(parcoursId, ueId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email == null) throw new UnauthorizedAccessException();

            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) throw new UnauthorizedAccessException();

            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null) throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) throw new UnauthorizedAccessException();
        }
    }
}
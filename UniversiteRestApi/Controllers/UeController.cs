using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Update;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UeDto>>> GetAsync()
        {
            string role = "", email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var uc = new GetTousLesUesUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                var ues = await uc.ExecuteAsync();
                return UeDto.ToDtos(ues);
            }
            catch
            {
                return ValidationProblem();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UeDto>> GetUnUe(long id)
        {
            string role = "", email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var uc = new GetUeByIdUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                var ue = await uc.ExecuteAsync(id);
                if (ue == null) return NotFound();
                return new UeDto().ToDto(ue);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<UeDto>> PostAsync([FromBody] UeDto ueDto)
        {
            string role = "", email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var uc = new CreateUeUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                var created = await uc.ExecuteAsync(ueDto.ToEntity());
                var dto = new UeDto().ToDto(created);
                return CreatedAtAction(nameof(GetUnUe), new { id = dto.Id }, dto);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(long id, [FromBody] UeDto ueDto)
        {
            if (id != ueDto.Id) return BadRequest();

            string role = "", email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var uc = new UpdateUeUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(ueDto.ToEntity());
                return NoContent();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(long id)
        {
            string role = "", email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var uc = new DeleteUeUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;

            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;

            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) throw new UnauthorizedAccessException();

            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null) throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
        }
    }
}
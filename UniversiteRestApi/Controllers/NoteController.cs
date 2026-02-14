using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Delete;
using UniversiteDomain.UseCases.NoteUseCases.Get;
using UniversiteDomain.UseCases.NoteUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet("etudiant/{etudiantId}")]
        public async Task<ActionResult<List<NoteAvecUeDto>>> GetNotesEtudiantAsync(long etudiantId)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }
            
            GetNoteEtudiantUseCase uc = new GetNoteEtudiantUseCase(repositoryFactory);

            if (!uc.IsAuthorized(role, user, etudiantId)) return Unauthorized();

            List<Note> notes;
            try
            {
                notes = await uc.ExecuteAsync(etudiantId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoteAvecUeDto.ToDtos(notes);
        }
        
        [HttpGet("{etudiantId}/{ueId}", Name = "GetNote")]
        public async Task<ActionResult<NoteAvecUeDto>> GetNoteAsync(long etudiantId, long ueId)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            if (!(role == Roles.Responsable || role == Roles.Scolarite ||
                  (role == Roles.Etudiant && user?.EtudiantId == etudiantId)))
                return Unauthorized();

            try
            {
                var notes = await repositoryFactory.NoteRepository().GetByEtudiantIdWithUeAsync(etudiantId);
                var note = notes.FirstOrDefault(n => n.EtudiantId == etudiantId && n.UeId == ueId);

                if (note == null) return NotFound();

                return new NoteAvecUeDto().ToDto(note);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<NoteAvecUeDto>> PostAsync([FromBody] CreateNoteDto dto)
        {
            CreateNoteUseCase uc = new CreateNoteUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            Note created;
            try
            {
                created = await uc.ExecuteAsync(dto.EtudiantId, dto.UeId, dto.Valeur);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            // Reload avec UE incluse pour renvoyer un DTO complet
            Note noteComplete = created;
            try
            {
                var notesAvecUe = await repositoryFactory.NoteRepository().GetByEtudiantIdWithUeAsync(created.EtudiantId);
                var found = notesAvecUe.FirstOrDefault(n => n.EtudiantId == created.EtudiantId && n.UeId == created.UeId);
                if (found != null) noteComplete = found;
            }
            catch { /* on ne bloque pas */ }

            var dtoOut = new NoteAvecUeDto().ToDto(noteComplete);

            // ✅ Là, ça match toujours : route GET /api/Note/{etudiantId}/{ueId}
            return CreatedAtRoute("GetNote",
                new { etudiantId = dtoOut.EtudiantId, ueId = dtoOut.UeId },
                dtoOut);
        }

        [HttpPut("{etudiantId}/{ueId}")]
        public async Task<ActionResult> PutAsync(long etudiantId, long ueId, [FromBody] float valeur)
        {
            UpdateNoteUseCase uc = new UpdateNoteUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(etudiantId, ueId, valeur);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        [HttpDelete("{etudiantId}/{ueId}")]
        public async Task<ActionResult> DeleteAsync(long etudiantId, long ueId)
        {
            DeleteNoteUseCase uc = new DeleteNoteUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(etudiantId, ueId);
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
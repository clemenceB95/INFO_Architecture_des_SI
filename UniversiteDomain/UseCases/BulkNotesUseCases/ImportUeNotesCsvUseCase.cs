using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.BulkNotes;
using UniversiteDomain.Dtos;

namespace UniversiteDomain.UseCases.BulkNotesUseCases;

public class ImportUeNotesCsvUseCase(
    IBulkNotesReader reader,
    IBulkNotesWriter writer,
    IUnitOfWork uow)
{
    public async Task<BulkNotesImportResultDto> ExecuteAsync(long ueId, Stream stream)
    {
        var res = new BulkNotesImportResultDto();

        var ue = await reader.GetUeAsync(ueId);
        if (ue == null)
        {
            res.Errors.Add(new BulkNotesImportErrorDto { Line = 0, Message = "UE introuvable." });
            return res;
        }

        var etudiants = await reader.GetEtudiantsFollowingUeAsync(ueId);
        var allowed = etudiants.ToDictionary(e => e.NumEtud, e => e.Id, StringComparer.OrdinalIgnoreCase);

        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null,
            BadDataFound = null
        };

        List<BulkNotesCsvRowDto> rows;
        using (var sr = new StreamReader(stream))
        using (var csv = new CsvReader(sr, cfg))
            rows = csv.GetRecords<BulkNotesCsvRowDto>().ToList();

        if (rows.Count == 0)
        {
            res.Errors.Add(new BulkNotesImportErrorDto { Line = 0, Message = "CSV vide." });
            return res;
        }

        // Validation complète
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < rows.Count; i++)
        {
            var line = i + 2;
            var r = rows[i];

            if (string.IsNullOrWhiteSpace(r.NumEtud))
                res.Errors.Add(new BulkNotesImportErrorDto { Line = line, Message = "NumEtud manquant." });

            if (!string.Equals(r.NumeroUe?.Trim(), ue.NumeroUe, StringComparison.OrdinalIgnoreCase))
                res.Errors.Add(new BulkNotesImportErrorDto { Line = line, Message = $"NumeroUe incorrect (attendu {ue.NumeroUe})." });

            if (!seen.Add(r.NumEtud))
                res.Errors.Add(new BulkNotesImportErrorDto { Line = line, Message = $"Doublon NumEtud {r.NumEtud}." });

            if (!allowed.ContainsKey(r.NumEtud))
                res.Errors.Add(new BulkNotesImportErrorDto { Line = line, Message = $"Etudiant {r.NumEtud} ne suit pas l'UE." });

            if (r.Note.HasValue && (r.Note.Value < 0f || r.Note.Value > 20f))
                res.Errors.Add(new BulkNotesImportErrorDto { Line = line, Message = $"Note invalide {r.Note} (0..20)." });
        }

        if (res.Errors.Count > 0)
        {
            res.Success = false;
            return res;
        }

        await uow.BeginTransactionAsync();
        try
        {
            int saved = 0;

            foreach (var r in rows)
            {
                if (!r.Note.HasValue) continue;
                var etudId = allowed[r.NumEtud];
                await writer.UpsertNoteAsync(etudId, ueId, r.Note.Value);
                saved++;
            }

            await uow.CommitAsync();
            res.Success = true;
            res.SavedCount = saved;
            return res;
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync();
            res.Errors.Add(new BulkNotesImportErrorDto { Line = 0, Message = "Erreur: " + ex.Message });
            res.Success = false;
            return res;
        }
    }
}
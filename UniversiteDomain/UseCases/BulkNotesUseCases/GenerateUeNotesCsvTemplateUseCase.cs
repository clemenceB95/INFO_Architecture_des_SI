using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.BulkNotes;
using UniversiteDomain.Dtos;

namespace UniversiteDomain.UseCases.BulkNotesUseCases;

public class GenerateUeNotesCsvTemplateUseCase(IBulkNotesReader reader)
{
    public async Task<byte[]> ExecuteAsync(long ueId)
    {
        var ue = await reader.GetUeAsync(ueId);
        if (ue == null) throw new Exception("UE introuvable");

        var etudiants = await reader.GetEtudiantsFollowingUeAsync(ueId);
        var notes = await reader.GetExistingNotesByUeAsync(ueId);

        var rows = etudiants.Select(e => new BulkNotesCsvRowDto
        {
            NumEtud = e.NumEtud,
            Nom = e.Nom,
            Prenom = e.Prenom,
            NumeroUe = ue.NumeroUe,
            Intitule = ue.Intitule,
            Note = notes.TryGetValue(e.NumEtud, out var v) ? v : null
        }).ToList();

        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms, new UTF8Encoding(true));
        using var csv = new CsvWriter(sw, cfg);

        await csv.WriteRecordsAsync(rows);
        await sw.FlushAsync();
        return ms.ToArray();
    }
}
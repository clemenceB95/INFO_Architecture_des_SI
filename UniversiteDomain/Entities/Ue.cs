namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;  // Exemple : MATH101
    public string NomUe { get; set; } = string.Empty; // Exemple : Algorithmique
}
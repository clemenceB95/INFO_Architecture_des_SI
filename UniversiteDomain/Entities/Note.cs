namespace UniversiteDomain.Entities;

public class Note
{
    public float Valeur { get; set; }

    // Clé composite + FK
    public long EtudiantId { get; set; }
    public long UeId { get; set; }

    // Navigations (OBLIGATOIRES pour le mapping du cours)
    public Etudiant? Etudiant { get; set; }
    public Ue? Ue { get; set; }
}
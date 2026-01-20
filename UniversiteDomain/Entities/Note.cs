namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }          
    public float Valeur { get; set; }     

    // Association Etudiant <-> UE
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
}
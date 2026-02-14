namespace UniversiteDomain.Dtos;

public class CreateNoteDto
{
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    public float Valeur { get; set; }
}
namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = String.Empty;
    public int AnneeFormation { get; set; } = 1;

    public List<Etudiant> Inscrits { get; set; } = new();
    public List<Ue> UesEnseignees { get; set; } = new();

    public override string ToString()
    {
        return "ID " + Id + " : " + NomParcours + " - Master " + AnneeFormation;
    }
}
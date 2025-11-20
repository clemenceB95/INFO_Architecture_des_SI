namespace UniversiteDomain.Exceptions.UeExceptions;

[Serializable]
public class InvalidUeNameException : Exception
{
    public InvalidUeNameException() 
        : base("Le nom d'une UE doit contenir plus de 3 caractères.") { }
}
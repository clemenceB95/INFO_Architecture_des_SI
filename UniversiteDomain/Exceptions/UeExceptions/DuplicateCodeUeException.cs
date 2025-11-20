namespace UniversiteDomain.Exceptions.UeExceptions;

[Serializable]
public class DuplicateCodeUeException : Exception
{
    public DuplicateCodeUeException(string code)
        : base($"Une UE existe déjà avec le code : {code}") { }
}
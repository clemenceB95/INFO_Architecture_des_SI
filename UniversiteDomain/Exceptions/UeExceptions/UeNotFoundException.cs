using System;

namespace UniversiteDomain.Exceptions.UeExceptions;

[Serializable]
public class UeNotFoundException : Exception
{
    public UeNotFoundException(string idUe)
        : base($"UE introuvable (id = {idUe})") { }
}
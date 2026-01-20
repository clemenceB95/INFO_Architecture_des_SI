using System;

namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class DuplicateUeDansParcoursException : Exception
{
    public DuplicateUeDansParcoursException(string message)
        : base(message)
    {
    }
}
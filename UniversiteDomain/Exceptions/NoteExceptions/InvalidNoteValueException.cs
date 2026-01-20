using System;

namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class InvalidNoteValueException : Exception
{
    public InvalidNoteValueException(float valeur)
        : base($"La note doit être comprise entre 0 et 20 (valeur = {valeur}).") { }
}
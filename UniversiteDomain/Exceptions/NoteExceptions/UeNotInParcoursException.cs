using System;

namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException(long idEtudiant, long idUe)
        : base($"L'étudiant {idEtudiant} ne peut pas être noté sur l'UE {idUe} car elle n'est pas dans son parcours.") { }
}
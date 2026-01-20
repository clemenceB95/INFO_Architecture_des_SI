using System;

namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class DuplicateNoteException : Exception
{
    public DuplicateNoteException(long idEtudiant, long idUe)
        : base($"L'étudiant {idEtudiant} a déjà une note pour l'UE {idUe}.") { }
}
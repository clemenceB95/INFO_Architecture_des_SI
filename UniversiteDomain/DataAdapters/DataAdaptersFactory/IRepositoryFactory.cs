namespace UniversiteDomain.DataAdapters.DataAdaptersFactory;
 
 public interface IRepositoryFactory
 {
     IParcoursRepository ParcoursRepository();
     IEtudiantRepository EtudiantRepository();
     IUeRepository UeRepository(); 
     // Méthodes de gestion de la dadasource
     // Ce sont des méthodes qui permettent de gérer l'ensemble des data source
     // comme tout supprimer ou tout créer
     Task EnsureDeletedAsync();
     Task EnsureCreatedAsync();
     Task SaveChangesAsync();
 }
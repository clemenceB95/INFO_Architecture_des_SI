using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository
{
    // Création d’une UE
    Task<Ue> CreateAsync(Ue ue);
    
    // Recherche d’une UE par son Id
    Task<Ue?> GetByIdAsync(long id);
    
    // Recherche conditionnelle (ex: par code)
    Task<List<Ue>> FindByConditionAsync(Expression<Func<Ue, bool>> predicate);
    
    // Récupération de toutes les UEs
    Task<List<Ue>> GetAllAsync();
    
    // Mise à jour
    Task<Ue> UpdateAsync(Ue ue);
    
    // Suppression
    Task DeleteAsync(long id);
}
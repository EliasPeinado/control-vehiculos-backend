using System.Linq.Expressions;

namespace ControlVehiculos.Repositories.Interfaces;

/// <summary>
/// Interfaz genérica del patrón Repository
/// Define las operaciones CRUD básicas para cualquier entidad
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IRepository<T> where T : class
{
    // READ Operations
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
    // CREATE Operation
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // UPDATE Operation
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    
    // DELETE Operation
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    
    // PAGINATION
    Task<(IEnumerable<T> Items, int Total)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true);
}

using System.Linq.Expressions;

namespace timetrace.library.Repositories;

public interface IRepositoryBase
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    TE Add<TE>(TE entity) where TE : class;

    /// <summary>
    /// Adds a list of entities to the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="entities">The list of entities to add.</param>
    /// <returns>The added entities.</returns>
    bool AddAll<TE>(IEnumerable<TE> entities) where TE : class;

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    TE Update<TE>(TE entity) where TE : class;

    /// <summary>
    /// Updates multiple entities in the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="entities">The entities to update.</param>
    /// <remarks>Use with caution, as this method will update all entities in the repository.</remarks>
    void UpdateAll<TE>(IEnumerable<TE> entities) where TE : class;

    /// <summary>
    /// Deletes a specific entity from the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="entity">The entity to delete.</param>
    void Delete<TE>(TE entity) where TE : class;

    /// <summary>
    /// Deletes multiple entities from the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    void DeleteAll<TE>(IEnumerable<TE> entities) where TE : class;

    /// <summary>
    /// Deletes entities from the repository based on a specified condition.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="expression">The condition to match.</param>
    void DeleteAll<TE>(Expression<Func<TE, bool>> expression) where TE : class;

    /// <summary>
    /// Deletes all entities from the repository.
    /// </summary>
    /// <typeparam name="TE"></typeparam>
    /// <returns>True if all entities were deleted successfully, otherwise throws an error.</returns>
    bool DeleteAll<TE>() where TE : class;

    /// <summary>
    /// Checks if any entity exists in the repository based on a specified condition.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="expression">The condition to match.</param>
    /// <returns>True if any entity exists, otherwise false.</returns>
    bool Exists<TE>(Expression<Func<TE, bool>> expression) where TE : class;

    /// <summary>
    /// Checks if a specific entity exists in the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    bool Exists<TE>(TE entity) where TE : class;

    /// <summary>
    /// Finds a single entity from the repository based on a specified condition.
    /// </summary>
    /// <typeparam name="TE">The type of the entity.</typeparam>
    /// <param name="expression">The condition to match.</param>
    /// <returns>The found entity, or null if not found.</returns>
    TE? Find<TE>(Expression<Func<TE, bool>> expression) where TE : class;

    /// <summary>
    /// Finds multiple entities from the repository based on a specified condition.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="expression">The condition to match.</param>
    /// <returns>An enumerable collection of the found entities.</returns>
    IEnumerable<TE> FindAll<TE>(Expression<Func<TE, bool>> expression) where TE : class;

    /// <summary>
    /// Fetches all entities from the repository.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <returns>An enumerable collection of all entities.</returns>
    IEnumerable<TE> FetchAll<TE>() where TE : class;

    /// <summary>
    /// Finds multiple entities from the repository based on a specified condition with pagination support.
    /// </summary>
    /// <typeparam name="TE">The type of the entities.</typeparam>
    /// <param name="expression">The condition to match.</param>
    /// <param name="pageSize">The maximum number of entities per page.</param>
    /// <param name="page">The page number.</param>
    /// <returns>An enumerable collection of the found entities.</returns>
    List<TE> FindAll<TE>(Expression<Func<TE, bool>> expression, int pageSize, int page) where TE : class;
}

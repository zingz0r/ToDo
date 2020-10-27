using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToDo.Persistence.Repositories
{
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Execute Query Transaction asynchronously
        /// </summary>
        /// <param name="execute">Query transaction lambda to execute</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>TResult</returns>
        Task<TResult> QueryAsync<TResult>(Func<Task<TResult>> execute, CancellationToken cancellationToken);

        /// <summary>
        /// Execute Transaction asynchronously
        /// </summary>
        /// <param name="execute">Transaction lambda to execute</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task ExecuteAsync(Func<Task> execute, CancellationToken cancellationToken);

        /// <summary>
        /// Add item to the db asynchronously
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task AddAsync(TEntity item, CancellationToken cancellationToken);

        /// <summary>
        /// Get multiple items from the db asynchronously in a range
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <param name="orderBy">Order by expression</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="max">Maximum number of entities. -1 = all</param>
        /// <param name="skip">Skip a number of entities.</param>
        /// <returns>TEntity the found items</returns>
        Task<IEnumerable<TEntity>> GetAsync<TKey>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> orderBy, CancellationToken ct, int max = -1, int skip = -1);

        /// <summary>
        /// Modify multiple items in the db asynchronously
        /// </summary>
        /// <param name="predicate">Predicate to select items</param>
        /// <param name="how">How to modify them</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>TEntity the modified items</returns>
        Task<IEnumerable<TEntity>> ModifyAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> how, CancellationToken cancellationToken);


        /// <summary>
        /// Delete multiple items from the db asynchronously 
        /// </summary>
        /// <param name="predicate">Predicate to select items</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>TEntity the deleted items</returns>
        Task<IEnumerable<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    }
}
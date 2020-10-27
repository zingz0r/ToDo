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
        /// Modify multiple items in the db asynchronously
        /// </summary>
        /// <param name="predicate">Predicate to select items</param>
        /// <param name="how">How to modify them</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Modified items</returns>
        Task<IEnumerable<TEntity>> ModifyAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> how, CancellationToken cancellationToken);


        /// <summary>
        /// Delete multiple items from the db asynchronously 
        /// </summary>
        /// <param name="predicate">Predicate to select items</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deleted items</returns>
        Task<IEnumerable<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    }
}
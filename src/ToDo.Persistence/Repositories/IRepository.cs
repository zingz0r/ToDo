using System;
using System.Threading;
using System.Threading.Tasks;

namespace ToDo.Persistence.Repositories
{
    public interface IRepository<in TEntity>
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
    }
}
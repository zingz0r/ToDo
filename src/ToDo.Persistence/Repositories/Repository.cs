using NHibernate;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Persistence.TransactionManager;

namespace ToDo.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly ITransactionManager _txManager;

        public Repository(ISession session, ILogger logger, ITransactionManager txManager)
        {
            _session = session;
            _logger = logger;
            _txManager = txManager;
        }

        public async Task<TResult> QueryAsync<TResult>(Func<Task<TResult>> execute, CancellationToken ct)
        {
            try
            {
                _txManager.BeginTransaction();
                var res = await execute.Invoke().ConfigureAwait(false);
                await _txManager.CommitTxAsync(ct).ConfigureAwait(false);

                return res;
            }
            catch (Exception)
            {
                await _txManager.RollbackTxAsync(ct).ConfigureAwait(false);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<Task> execute, CancellationToken cancellationToken)
        {
            try
            {
                _txManager.BeginTransaction();
                await execute.Invoke().ConfigureAwait(false);
                await _txManager.CommitTxAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await _txManager.RollbackTxAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public virtual async Task AddAsync(TEntity item, CancellationToken ct)
        {
            _txManager.EnsureTransactionIsAlive();

            _logger.Verbose("{function} Adding {@item}", $"{GetInterfaceName()}.{nameof(AddAsync)}", item);

            await _session.SaveAsync(item, ct).ConfigureAwait(false);
        }

        private string GetInterfaceName()
        {
            const string iName = "IRepository";
            return $"{iName}<{GetType().GetInterfaces().First(i => i.Name.Contains(iName)).GenericTypeArguments[0].Name}>";
        }
    }
}
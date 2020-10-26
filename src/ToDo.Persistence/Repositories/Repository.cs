using NHibernate;
using NHibernate.Linq;
using Serilog;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Persistence.Extensions;
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

        public async Task AddAsync(TEntity item, CancellationToken ct)
        {
            _txManager.EnsureTransactionIsAlive();

            _logger.Verbose("{function} Adding {@item}", $"{GetInterfaceName()}.{nameof(AddAsync)}", item);

            await _session.SaveAsync(item, ct).ConfigureAwait(false);
        }

        public async Task ModifyAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> how, CancellationToken ct)
        {
            _txManager.EnsureTransactionIsAlive();

            _logger.Verbose("{function} Modifying items with [Predicate({@predicate})]",
                $"{GetInterfaceName()}.{nameof(ModifyAsync)}", predicate.ToReadableString());

            var items = await _session.Query<TEntity>().Where(predicate).ToListAsync(ct);
            if (!items.Any())
            {
                throw new Exception($"{GetInterfaceName()}.{nameof(ModifyAsync)}: Not found any item matching [Predicate({predicate.ToReadableString()})]!");
            }

            foreach (var item in items)
            {
                how.Invoke(item);
                await _session.UpdateAsync(item, ct);
            }
        }

        private string GetInterfaceName()
        {
            const string iName = "IRepository";
            return $"{iName}<{GetType().GetInterfaces().First(i => i.Name.Contains(iName)).GenericTypeArguments[0].Name}>";
        }
    }
}
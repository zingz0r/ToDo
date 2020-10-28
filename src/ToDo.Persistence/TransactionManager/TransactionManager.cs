using NHibernate;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ToDo.Persistence.TransactionManager
{
    public class TransactionManager : ITransactionManager, IDisposable
    {
        private readonly ISession _session;
        private readonly ILogger _logger;
        private ITransaction _transaction;

        public TransactionManager(ISession session, ILogger logger)
        {
            _session = session;
            _logger = logger.ForContext<TransactionManager>();
        }

        public void BeginTransaction()
        {
            if (_transaction == null || !_transaction.IsActive)
                _transaction = _session.BeginTransaction();
        }

        public void EnsureTransactionIsAlive()
        {
            if (_transaction == null || !_transaction.IsActive)
                throw new TransactionException("Transaction is not active");
        }

        public async Task CommitTxAsync(CancellationToken ct = default)
        {
            if (_transaction != null && !_transaction.WasCommitted)
            {
                await _session.FlushAsync(ct).ConfigureAwait(false);
                _session.Clear();
                await _transaction.CommitAsync(ct).ConfigureAwait(false);
                _logger.Verbose("{manager} committed the transaction", GetType().Name);
            }
        }

        public async Task RollbackTxAsync(CancellationToken ct = default)
        {
            if (_transaction != null && _transaction.IsActive)
            {
                _logger.Verbose("{manager} rolled back the transaction", GetType().Name);
                await _session.FlushAsync(ct).ConfigureAwait(false);
                _session.Clear();
                await _transaction.RollbackAsync(ct).ConfigureAwait(false);
            }
        }

        public async void Dispose()
        {
            await RollbackTxAsync().ConfigureAwait(false);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace ToDo.Persistence.TransactionManager
{
    public interface ITransactionManager
    {
        void BeginTransaction();
        void EnsureTransactionIsAlive();
        Task CommitTxAsync(CancellationToken ct);
        Task RollbackTxAsync(CancellationToken ct);
    }
}
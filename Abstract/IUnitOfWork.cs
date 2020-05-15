namespace EFPostgreSupplier.Abstract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntityBase, new();
        
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        void RollbackTransaction();
        
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
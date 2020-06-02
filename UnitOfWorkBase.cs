namespace EFPostgresEngagement
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstract;
    using DbContextBase;

    public abstract class UnitOfWorkBase<TDbContext> : IUnitOfWork
        where TDbContext : PostgresDbContextBase<TDbContext>
    {
        protected readonly TDbContext DbContext;

        public UnitOfWorkBase(TDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public abstract IRepository<T> GetRepository<T>() where T : class, new();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await this.DbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public void CommitTransaction()
        {
            this.DbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            this.DbContext.Database.RollbackTransaction();
        }

        public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            return await this.DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            this.DbContext?.Dispose();
        }
    }
}
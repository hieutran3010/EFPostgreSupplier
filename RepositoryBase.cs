namespace EFPostgresEngagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstract;
    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryBase<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class, new()
        where TDbContext : PostgresDbContextBase<TDbContext>
    {
        protected readonly TDbContext DbContext;

        protected RepositoryBase(TDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public IQueryable<TEntity> GetQueryable(bool asNoTracking = true)
        {
            return asNoTracking
                ? this.DbContext.Set<TEntity>().AsNoTracking()
                : this.DbContext.Set<TEntity>();
        }

        public Task<TEntity> FindAsync(Guid id)
        {
            return this.DbContext.Set<TEntity>().FindAsync(id).AsTask();
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return this.DbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return this.DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        }

        public Task AddRangeAsync(params TEntity[] entities)
        {
            return this.DbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public Task<int> ExecuteSqlRawAsync(string sql)
        {
            return this.DbContext.Database.ExecuteSqlRawAsync(sql);
        }

        public IQueryable<TEntity> GetByRawSql(string sql)
        {
            return this.DbContext.Set<TEntity>().FromSqlRaw(sql);
        }

        /// <inheritdoc />
        public void Update(TEntity entity)
        {
            this.DbContext.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            this.DbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.DbContext.Set<TEntity>().RemoveRange(entities);
        }
    }
}
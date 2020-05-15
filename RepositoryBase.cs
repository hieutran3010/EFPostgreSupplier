namespace EFPostgresEngagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstract;
    using DbContextBase;
    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryBase<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class, IEntityBase, new()
        where TDbContext: PostgresDbContextBase<TDbContext>
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

        public async Task<TEntity> FindAsync(Guid id)
        {
            return await this.DbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await this.DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await this.DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        }

        public async Task AddRangeAsync(params TEntity[] entities)
        {
            await this.DbContext.Set<TEntity>().AddRangeAsync(entities);
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
namespace EFPostgresEngagement.DbContextBase
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstract;
    using DataAnnotationAttributes;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public abstract class PostgresDbContextBase<TContext>: DbContext where TContext: DbContext
    {
        protected IDbTracker DbTracker { get; set; }
        
        protected PostgresDbContextBase(DbContextOptions<TContext> options,
            IDbTracker dbTracker)
            : base(options)
        {
            this.DbTracker = dbTracker;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.OnExtendModelCreating(modelBuilder);
            foreach (var entityType in Assembly.GetExecutingAssembly().ExportedTypes
                .Where(o => o.BaseType == typeof(IEntityBase) && o.IsClass && !o.IsAbstract))
            {
                modelBuilder.Entity(entityType);
            }

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.HasPostgresExtension("citext");

            var internalModelBuilder = modelBuilder.GetInfrastructure();
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                this.ApplyDataAnnotationAttributesAndColumnTypes(modelBuilder, entityType.ClrType);
                
                internalModelBuilder.Entity(entityType.Name)
                    .ToTable(entityType.ClrType.Name.ToLowerInvariant());

                entityType.GetProperties().ToList().ForEach(o => o.SetColumnName(o.Name.ToLowerInvariant()));

                entityType.GetKeys().ToList()
                    .ForEach(o => o.SetName(o.GetName().ToLowerInvariant()));

                entityType.GetForeignKeys().ToList()
                    .ForEach(o => o.SetConstraintName(o.GetConstraintName().ToLowerInvariant()));

                entityType.GetIndexes().ToList()
                    .ForEach(o => o.SetDatabaseName(o.GetDatabaseName().ToLowerInvariant()));
            }

            base.OnModelCreating(modelBuilder);
        }

        public virtual void OnExtendModelCreating(ModelBuilder modelBuilder)
        {
            // do nothing
        }
        
        public override int SaveChanges()
        {
            this.UpdateTrackerInformation();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.UpdateTrackerInformation();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void UpdateTrackerInformation()
        {
            var addedEntities = this.ChangeTracker.Entries().Where(c => c.State == EntityState.Added)
                .Select(c => c.Entity).OfType<IEntityBase>();
            foreach (var entity in addedEntities)
            {
                entity.CreatedBy = this.DbTracker.GetAuthor();
                entity.ModifiedBy = this.DbTracker.GetAuthor();
            }

            var modifiedEntities = this.ChangeTracker.Entries().Where(c => c.State == EntityState.Modified)
                .Select(c => c.Entity).OfType<IEntityBase>();
            foreach (var entity in modifiedEntities)
            {
                entity.ModifiedBy = this.DbTracker.GetAuthor();
                entity.ModifiedOn = DateTime.Now;
            }
        }
        
        protected virtual void ApplyDataAnnotationAttributesAndColumnTypes(ModelBuilder modelBuilder, Type entityType)
        {
            var entityTypeBuilder = modelBuilder.Entity(entityType);

            var properties = entityType.GetProperties()
                .Where(o => o.PropertyType.IsValueType || o.PropertyType == typeof(string));
            foreach (var property in properties)
            {
                var propertyBuilder = entityTypeBuilder.Property(property.Name);
                if (property.PropertyType == typeof(string))
                {
                    propertyBuilder.HasColumnType("citext");
                }

                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    switch (attribute)
                    {
                        case KeyAttribute _:
                            propertyBuilder.HasDefaultValueSql("uuid_generate_v1mc()");
                            continue;
                        case RequiredAttribute requiredAttribute when requiredAttribute.AllowEmptyStrings:
                            propertyBuilder.HasDefaultValue(string.Empty);
                            continue;
                        case DefaultValueAttribute defaultValueAttribute:
                            propertyBuilder.HasDefaultValue(defaultValueAttribute.Value);
                            continue;
                        case DateTimeNowDefaultAttribute _:
                            propertyBuilder.HasDefaultValueSql("timezone('utc'::text, now())");
                            continue;
                        case SimpleIndexAttribute simpleIndexAttribute:
                            var simpleIndexPropertyNames = string.Concat(string.Join(",", property.Name),
                                ",", simpleIndexAttribute.AdditionalColumns ?? string.Empty);
                            entityTypeBuilder.HasIndex(simpleIndexPropertyNames.Split(',',
                                StringSplitOptions.RemoveEmptyEntries));
                            continue;
                        case UniqueIndexAttribute uniqueIndexAttribute:
                            var uniqueIndexPropertyNames = string.Concat(string.Join(",", property.Name),
                                ",", uniqueIndexAttribute.AdditionalColumns ?? string.Empty);
                            
                            entityTypeBuilder
                                .HasIndex(uniqueIndexPropertyNames.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                .IsUnique();
                            continue;
                    }
                }
            }
        }
    }
}
# Entity Framework & Postgres Engagement
This library provides the engagement between Entity Framework Core and PosgreSQL. In other hand, it gives some services for CRUD such as IUnitOfWork, IRepository,.. Just do a litte code and let's start
### Installation
This library requires .NET Core 3.x
##### Install via Package Manager
```sh
$ Install-Package EFPostgresEngagement
```
##### Install via .NET CLI
```sh
$ dotnet add package EFPostgresEngagement
```
# How to use?

  - Create a new environment variable for the Postgres connection
```sh
CONNECTION_STRING: '....'
```
  - Create an entity base and let's in inherits DefaultEntityBase
```c#
    public class EntityBase : DefaultEntityBase
    {
    }
```
  - Create a new DbContext and let's it inherits PostgresDbContextBase
```c#
    public class MyDataDbContext : PostgresDbContextBase<MyDataDbContext>
    {
        public MyDataDbContext(DbContextOptions<MyDataDbContext> options, IDbTracker dbTracker) : base(options,
            dbTracker)
        {
        }
    }
```
  - Create class Repository and let's it inherits RepostoryBase
```c#
    public class Repository<TEntity> : RepositoryBase<TEntity, MyDataDbContext>
        where TEntity : class, IEntityBase, new()
    {
        public Repository(AirReturnDbContext dbContext) : base(dbContext)
        {
        }
    }
```
  - Create class UnitOfWork and let's it inherits UnitOfWorkBase
```c#
    public class UnitOfWork : UnitOfWorkBase<MyDataDbContext>
    {
        public UnitOfWork(MyDataDbContext dbContext) : base(dbContext)
        {
        }

        public override IRepository<T> GetRepository<T>()
        {
            return new Repository<T>(this.DbContext);
        }
    }
```
  - Finally, registering all services in StartUp file
```c#
    services
        .UsePostgresSql<AirReturnDbContext>(this.Configuration)
        .AddTransient<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IDbTracker>(provider => new ApplicationUserProvider());
```
*Note: IDbTracker helps to track the owner of data modification. In the near future, it will be optional. Temporarily for now, please implement a class to pass this requirement* 

### An example project is implementing. Please watch for new update.

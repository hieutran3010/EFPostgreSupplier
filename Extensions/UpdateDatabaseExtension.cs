namespace EFPostgreSupplier.Extensions
{
    using DbContextBase;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Npgsql;

    public static class UpdateDatabaseExtension
    {
        public static void UpdateDatabase<TDbContext>(this IApplicationBuilder app)
            where TDbContext : PostgresDbContextBase<TDbContext>
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<TDbContext>();
            context.Database.Migrate();
            context.Database.OpenConnection();
            ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();
            context.Database.CloseConnection();
        }
    }
}
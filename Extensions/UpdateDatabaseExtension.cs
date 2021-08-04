namespace EFPostgresEngagement.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class UpdateDatabaseExtension
    {
        public static void UpdateDatabase<TDbContext>(this IApplicationBuilder app)
            where TDbContext : PostgresDbContextBase<TDbContext>
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<TDbContext>();
            context.Database.Migrate();
        }
    }
}
namespace EFPostgresEngagement.Extensions
{
    using System;
    using DbContextBase;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class PostgresProviderExtension
    {
        public static IServiceCollection UsePostgresSql<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext: PostgresDbContextBase<TDbContext>
        {
            services.AddEntityFrameworkNpgsql()
            .AddDbContext<TDbContext>(option =>
            {
#if DEBUG
                option.UseLoggerFactory(GetLoggerFactory());
#endif
                var connection = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                option.UseNpgsql(connection ??
                                 throw new InvalidOperationException(
                                     "Cannot find the CONNECTION_STRING in environment variables"));
            });
            return services;
        }

        private static ILoggerFactory GetLoggerFactory()
        {
            return new ServiceCollection().AddLogging(builder => builder
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information))
                .BuildServiceProvider()
                .GetService<ILoggerFactory>();
        }
    }
}

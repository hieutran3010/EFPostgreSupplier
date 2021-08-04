namespace EFPostgresEngagement.Extensions
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class PostgresProviderExtension
    {
        public static IServiceCollection UsePostgresSql<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, bool isDevelopment = false, Action<DbContextOptionsBuilder> config = null)
            where TDbContext : PostgresDbContextBase<TDbContext>
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<TDbContext>(option =>
                {
                    if (isDevelopment)
                    {
                        option.UseLoggerFactory(Factory);
                        option.EnableSensitiveDataLogging();
                    }

                    var connection = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                    option.UseNpgsql(connection ??
                                     throw new InvalidOperationException(
                                         "Cannot find the CONNECTION_STRING in environment variables"));
                    if (config != null)
                    {
                        config(option);
                    }
                });
            return services;
        }

        private static readonly ILoggerFactory Factory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });
    }
}
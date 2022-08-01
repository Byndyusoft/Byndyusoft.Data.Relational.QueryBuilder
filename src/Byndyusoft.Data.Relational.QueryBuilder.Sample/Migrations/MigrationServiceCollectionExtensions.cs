using Byndyusoft.Data.Relational.QueryBuilder.Sample.HostedServices;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Migrations
{
    public static class MigrationServiceCollectionExtensions
    {
        public static IServiceCollection AddMigration(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("postgres");

            return serviceCollection
                .AddFluentMigratorCore()
                .ConfigureRunner(runner =>
                {
                    runner.AddPostgres()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(typeof(Program).Assembly).For.Migrations();
                })
                .AddLogging(log => log.AddFluentMigratorConsole())
                .AddHostedService<MigrationHostedService>();
        }
    }
}

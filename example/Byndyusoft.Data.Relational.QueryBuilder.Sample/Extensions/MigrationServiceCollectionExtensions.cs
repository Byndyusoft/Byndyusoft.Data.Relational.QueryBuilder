using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.HostedServices;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSampleApplication(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                .AddMigration(configuration)
                .AddApplication();
        }

        private static IServiceCollection AddMigration(this IServiceCollection serviceCollection,
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

        private static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<CompanyRepository>()
                .AddSingleton<UserDtoRepository>()
                .AddSingleton<CompanyReportDtoRepository>();
        }
    }
}
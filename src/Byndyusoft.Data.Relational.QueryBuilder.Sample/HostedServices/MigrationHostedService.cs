using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#pragma warning disable 1998

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.HostedServices
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MigrationHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var migrationRunner = serviceScope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            migrationRunner.MigrateUp();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}

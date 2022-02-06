using Byndyusoft.Data.Relational.QueryBuilder.Example.Entities;
using Byndyusoft.Data.Relational.QueryBuilder.Example.QueryBuilders;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Byndyusoft.Data.Relational.QueryBuilder.Example
{
    public static class Program
    {
        public static async Task Main()
        {
            var file = "test.db";

            File.Delete(file);

            await File.Create(file).DisposeAsync();

            var serviceProvider =
                new ServiceCollection()
                    .AddRelationalDb(SqliteFactory.Instance, $"data source={file}")
                    .BuildServiceProvider();

            await InitDbAsync(serviceProvider);

            await ExampleInsertAsync(serviceProvider);

            await ExampleSelectAsync(serviceProvider);
        }
        private static async Task ExampleInsertAsync(ServiceProvider serviceProvider)
        {
            var sessionFactory = serviceProvider.GetRequiredService<IDbSessionFactory>();
            await using var session = await sessionFactory.CreateCommittableSessionAsync();

            var entity = new Entity
            {
                City = "Chelyabinsk",
                Birthday = "2021-05-07",
                Name = "name4",
                Id = 4
            };

            var insertQuery = new EntityInsertQuery(entity).AllPublicValues().Build();

            Console.WriteLine("=== INSERT SQL:");
            Console.WriteLine(insertQuery.Sql);
            Console.WriteLine("=== INSERT PARAMS:");
            Console.WriteLine(JsonConvert.SerializeObject(insertQuery.Params));

            await session.ExecuteAsync(insertQuery);

            await session.CommitAsync();
        }

        private static async Task ExampleSelectAsync(ServiceProvider serviceProvider)
        {
            var sessionFactory = serviceProvider.GetRequiredService<IDbSessionFactory>();
            await using var session = await sessionFactory.CreateSessionAsync();

            var selectQuery = new EntitySelectQuery().ByCity("Chelyabinsk").Build();

            Console.WriteLine("=== SELECT SQL:");
            Console.WriteLine(selectQuery.Sql);
            Console.WriteLine("=== SELECT PARAMS:");
            Console.WriteLine(JsonConvert.SerializeObject(selectQuery.Params));

            Console.WriteLine("=== ROWS:");

            var result = session.Query<Entity>(selectQuery);
            await foreach (var row in result) Console.WriteLine(JsonConvert.SerializeObject(row));
        }

        private static async Task InitDbAsync(IServiceProvider serviceProvider)
        {
            var sessionFactory = serviceProvider.GetRequiredService<IDbSessionFactory>();
            await using var session = await sessionFactory.CreateCommittableSessionAsync();
            await session.ExecuteAsync("CREATE TABLE entities (id PRIMARY KEY ASC, name TEXT, birthday TEXT, city TEXT)");

            await session.ExecuteAsync(
                "INSERT INTO entities (id, name, birthday, city) VALUES (1, 'name1', '2021-05-08', 'Chelyabinsk');");
            await session.ExecuteAsync(
                "INSERT INTO entities (id, name, birthday, city) VALUES (2, 'name2', '1998-01-02', 'Chelyabinsk');");
            await session.ExecuteAsync(
                "INSERT INTO entities (id, name, birthday, city) VALUES (3, 'name3', '2011-09-13', 'Moscow');");

            await session.CommitAsync();
        }
    }
}

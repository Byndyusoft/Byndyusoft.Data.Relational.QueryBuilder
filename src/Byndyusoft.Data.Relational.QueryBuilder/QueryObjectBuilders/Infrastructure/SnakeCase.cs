using System.Collections.Concurrent;
using Npgsql.NameTranslation;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public static class SnakeCase
    {
        private static readonly ConcurrentDictionary<string, string> SnakeCases = new();

        public static string Get(string text)
        {
            if (SnakeCases.TryGetValue(text, out var result))
                return result;

            result = NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(text);
            SnakeCases.TryAdd(text, result);

            return result;
        }
    }
}
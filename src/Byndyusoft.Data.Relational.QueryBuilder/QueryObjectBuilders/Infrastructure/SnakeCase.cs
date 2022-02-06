using Npgsql.NameTranslation;
using System.Collections.Concurrent;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public static class SnakeCase
    {
        private static readonly ConcurrentDictionary<string, string> SnakeCahse =
            new ConcurrentDictionary<string, string>();

        public static string Get(string text)
        {
            if (SnakeCahse.TryGetValue(text, out var result))
                return result;

            result = NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(text);
            SnakeCahse.TryAdd(text, result);

            return result;
        }
    }
}
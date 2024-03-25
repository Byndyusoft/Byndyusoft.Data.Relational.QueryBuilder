using System.Collections.Generic;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class QueryObjectBuilderExtensions
    {
        public static IEnumerable<TypePropertyInfo<T>> ExcludeId<T>(this IEnumerable<TypePropertyInfo<T>> properties)
            where T : IEntity
        {
            return properties.Exclude(i => i.Id);
        }

        public static IEnumerable<string> ExcludeId(this IEnumerable<string> properties)
        {
            return properties.Exclude<IEntity, long>(i => i.Id);
        }
    }
}
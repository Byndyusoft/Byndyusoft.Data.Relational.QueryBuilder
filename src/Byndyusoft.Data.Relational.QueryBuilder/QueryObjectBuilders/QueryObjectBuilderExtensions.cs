using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;
using System.Collections.Generic;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class QueryObjectBuilderExtensions
    {
        public static IEnumerable<TypePropertyInfo> ExcludeId(this IEnumerable<TypePropertyInfo> properties)
        {
            return properties.Exclude<IEntity, long>(i => i.Id);
        }

        public static IEnumerable<string> ExcludeId(this IEnumerable<string> properties)
        {
            return properties.Exclude<IEntity, long>(i => i.Id);
        }
    }
}
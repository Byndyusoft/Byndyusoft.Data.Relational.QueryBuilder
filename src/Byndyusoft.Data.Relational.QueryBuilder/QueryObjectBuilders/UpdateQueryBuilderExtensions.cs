using System;
using System.Collections.Generic;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class UpdateQueryBuilderExtensions
    {
        public static UpdateItemQueryBuilder<T> UpdateAllPublicValues<T>(
            this UpdateItemQueryBuilder<T> builder,
            Func<IEnumerable<TypePropertyInfo>, IEnumerable<TypePropertyInfo>>? transformer = null)
            where T : IEntity
        {
            return builder.AllPublicValues(i =>
            {
                var properties = i.ExcludeId();
                if (transformer is not null)
                    properties = transformer(properties);

                return properties;
            });
        }
    }
}
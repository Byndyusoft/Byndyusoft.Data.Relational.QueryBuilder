using System;
using System.Collections.Generic;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class InsertQueryBuilderExtensions
    {
        public static InsertQueryBuilder<T> InsertAllPublicValues<T>(
            this InsertQueryBuilder<T> builder,
            Func<IEnumerable<TypePropertyInfo<T>>, IEnumerable<TypePropertyInfo<T>>>? transformer = null)
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
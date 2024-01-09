using System;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public class QueryBuilder<T>
    {
        protected readonly ColumnConverter ColumnConverter;

        private QueryBuilder(ColumnConverter columnConverter)
        {
            ColumnConverter = columnConverter;
        }

        public static QueryBuilder<T> For(ColumnConverter columnConverter)
        {
            return new QueryBuilder<T>(columnConverter);
        }

        public string Get(Expression<Func<T, string>> formatString, string? tableAlias = null)
        {
            return ColumnConverter.Map(formatString, tableAlias);
        }
    }
}
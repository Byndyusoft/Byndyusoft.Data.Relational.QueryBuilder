using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class InsertQueryBuilderExtensions
    {
        public static InsertQueryBuilder<T> InsertAllPublicValues<T>(this InsertQueryBuilder<T> builder)
            where T : IEntity
        {
            return builder.AllPublicValues(i => i.ExcludeId());
        }
    }
}
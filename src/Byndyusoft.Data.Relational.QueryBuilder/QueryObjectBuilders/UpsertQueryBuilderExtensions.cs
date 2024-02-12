using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class UpsertQueryBuilderExtensions
    {
        public static UpsertQueryBuilder<T> UpsertAllPublicValues<T>(this UpsertQueryBuilder<T> builder)
            where T : IEntity
        {
            return builder.AllPublicValues(i => i.ExcludeId());
        }
    }
}
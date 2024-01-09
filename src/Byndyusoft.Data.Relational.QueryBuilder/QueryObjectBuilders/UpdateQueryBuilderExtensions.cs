using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public static class UpdateQueryBuilderExtensions
    {
        public static UpdateItemQueryBuilder<T> UpdateAllPublicValues<T>(this UpdateItemQueryBuilder<T> builder)
            where T : IEntity
        {
            return builder.AllPublicValues(i => i.ExcludeId());
        }
    }
}
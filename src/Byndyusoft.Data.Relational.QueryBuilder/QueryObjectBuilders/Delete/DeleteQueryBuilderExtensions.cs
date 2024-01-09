using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Delete
{
    public static class DeleteQueryBuilderExtensions
    {
        public static TBuilder ById<TBuilder, T>(this DeleteQueryBuilderBase<TBuilder, T> builder, long id)
            where TBuilder : DeleteQueryBuilderBase<TBuilder, T>
            where T : IEntity
        {
            return builder.WhereEquals(i => i.Id, id);
        }
    }
}
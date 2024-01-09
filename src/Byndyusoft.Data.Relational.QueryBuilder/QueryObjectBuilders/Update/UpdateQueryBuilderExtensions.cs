using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update
{
    public static class UpdateQueryBuilderExtensions
    {
        public static TBuilder ById<TBuilder, TEntity>(this UpdateQueryBuilderBase<TBuilder, TEntity> builder, long id)
            where TBuilder : UpdateQueryBuilderBase<TBuilder, TEntity>
            where TEntity : IEntity
        {
            return builder.WhereEquals(i => i.Id, id);
        }

        public static TBuilder ById<TBuilder, TEntity>(this UpdateItemQueryBuilderBase<TBuilder, TEntity> builder)
            where TBuilder : UpdateItemQueryBuilderBase<TBuilder, TEntity>
            where TEntity : IEntity
        {
            return builder.ById(builder.Entity.Id);
        }
    }
}
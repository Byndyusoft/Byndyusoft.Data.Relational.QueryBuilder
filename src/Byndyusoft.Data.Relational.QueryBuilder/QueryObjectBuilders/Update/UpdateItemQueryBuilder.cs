namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update
{
    public class UpdateItemQueryBuilder<T> : UpdateItemQueryBuilderBase<UpdateItemQueryBuilder<T>, T>
    {
        protected UpdateItemQueryBuilder(T entity, string tableName, string? tableAlias, bool isPostgres) : base(entity,
            tableName,
            tableAlias, isPostgres)
        {
        }

        public static UpdateItemQueryBuilder<T> For(T entity, string tableName, string? tableAlias = null,
            bool isPostgres = true)
        {
            return new UpdateItemQueryBuilder<T>(entity, tableName, tableAlias, isPostgres);
        }
    }

    public static class UpdateItemQueryBuilder
    {
        public static UpdateItemQueryBuilder<T> For<T>(T entity, string tableName, string? tableAlias = null,
            bool isPostgres = true)
        {
            return UpdateItemQueryBuilder<T>.For(entity, tableName, tableAlias, isPostgres);
        }
    }
}
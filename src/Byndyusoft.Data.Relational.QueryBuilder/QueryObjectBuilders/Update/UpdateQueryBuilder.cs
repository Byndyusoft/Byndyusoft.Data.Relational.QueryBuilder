namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update
{
    public class UpdateQueryBuilder : UpdateQueryBuilderBase<UpdateQueryBuilder>
    {
        protected UpdateQueryBuilder(string tableName, string? tableAlias, bool isPostgres) : base(tableName,
            tableAlias, isPostgres)
        {
        }

        public static UpdateQueryBuilder For(string tableName, string? tableAlias = null, bool isPostgres = true)
        {
            return new UpdateQueryBuilder(tableName, tableAlias, isPostgres);
        }
    }

    public class UpdateQueryBuilder<T> : UpdateQueryBuilderBase<UpdateQueryBuilder<T>, T>
    {
        private UpdateQueryBuilder(string tableName, string? tableAlias, bool isPostgres) : base(tableName, tableAlias,
            isPostgres)
        {
        }

        public static UpdateQueryBuilder<T> For(string tableName, string? tableAlias = null, bool isPostgres = true)
        {
            return new UpdateQueryBuilder<T>(tableName, tableAlias, isPostgres);
        }
    }
}
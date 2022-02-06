namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Delete
{
    public class DeleteQueryBuilder : DeleteQueryBuilderBase<DeleteQueryBuilder>
    {
        private DeleteQueryBuilder(string tableName, bool isPostgres)
            : base(tableName, isPostgres)
        {
        }

        public static DeleteQueryBuilder For(string tableName, bool isPostgres = true)
        {
            return new DeleteQueryBuilder(tableName, isPostgres);
        }
    }

    public class DeleteQueryBuilder<T> : DeleteQueryBuilderBase<DeleteQueryBuilder<T>, T>
    {
        private DeleteQueryBuilder(string tableName, bool isPostgres)
            : base(tableName, isPostgres)
        {
        }

        public static DeleteQueryBuilder<T> For(string tableName, bool isPostgres = true)
        {
            return new DeleteQueryBuilder<T>(tableName, isPostgres);
        }
    }
}
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using System;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public class SimpleSelectBuilder
    {
        private readonly bool _isPostgres;
        private readonly string _tableName;
        private FormattableString? _whereSql;

        public SimpleSelectBuilder(string tableName, bool isPostgres)
        {
            _tableName = tableName;
            _isPostgres = isPostgres;
        }

        public static SimpleSelectBuilder From(string tableName, bool isPostgres = true)
        {
            return new SimpleSelectBuilder(tableName, isPostgres);
        }

        public SimpleSelectBuilder Where(FormattableString where)
        {
            _whereSql = where;
            return this;
        }

        public QueryObject Build()
        {
            var conditionals = new ConditionalCollector(new ColumnConverter(_isPostgres));

            var sql = $@"
SELECT * FROM {_tableName}";

            if (_whereSql != null)
            {
                conditionals.AddFormattableString(_whereSql);
                sql = sql + $@"
WHERE {conditionals.GetConditional()}";
            }

            return new QueryObject(sql, conditionals.GetParameters());
        }
    }
}
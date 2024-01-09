using System;
using System.Linq;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Dapper;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public class UpdateOrInsertQueryBuilder
    {
        private readonly ColumnConverter _columnConverter;
        private readonly ValueCollector _insertValueCollector;
        private readonly string _tableName;
        private readonly ValueCollector _upsertValueCollector;
        private FormattableString? _whereSql;

        public UpdateOrInsertQueryBuilder(string tableName, bool isPostgres)
        {
            _tableName = tableName;
            _columnConverter = new ColumnConverter(isPostgres);
            _insertValueCollector = new ValueCollector(_columnConverter, "@_iv");
            _upsertValueCollector = new ValueCollector(_columnConverter, "@_uv");
        }

        public static UpdateOrInsertQueryBuilder For(string tableName, bool isPostgres = true)
        {
            return new UpdateOrInsertQueryBuilder(tableName, isPostgres);
        }

        public UpdateOrInsertQueryBuilder Insert(object value, string fieldName)
        {
            _insertValueCollector.Add(fieldName, value);
            return this;
        }

        public UpdateOrInsertQueryBuilder Insert<T, TProp>(T item, Expression<Func<T, TProp>> action)
        {
            var fieldName = _columnConverter.ToColumnName(action);
            var value = ExpressionsCache<T, TProp>.Get(action)(item);

            _insertValueCollector.Add(fieldName, value);
            return this;
        }

        public UpdateOrInsertQueryBuilder Upsert<T, TProp>(T item, Expression<Func<T, TProp>> action)
        {
            var fieldName = _columnConverter.ToColumnName(action);
            var value = ExpressionsCache<T, TProp>.Get(action)(item);

            _upsertValueCollector.Add(fieldName, value);
            return this;
        }

        public UpdateOrInsertQueryBuilder Where(FormattableString where)
        {
            _whereSql = where;
            return this;
        }

        public QueryObject Build()
        {
            var allInsertParameters = _insertValueCollector.FieldParameters
                .Concat(_upsertValueCollector.FieldParameters).ToArray();
            var updateFields = _upsertValueCollector.FieldParameters.Select(x => $"{x.Field}={x.ParamName}");

            var conditionals = new ConditionalCollector(_columnConverter);
            if (_whereSql == null)
                throw new Exception("Where clause is null");
            conditionals.AddFormattableString(_whereSql);

            var sql = $@"
UPDATE {_tableName} SET {string.Join(", ", updateFields)}
WHERE {conditionals.GetConditional()};

INSERT INTO {_tableName}( {string.Join(", ", allInsertParameters.Select(x => x.Field))} )
SELECT {string.Join(",", allInsertParameters.Select(x => x.ParamName))} 
WHERE NOT EXISTS (SELECT * FROM {_tableName} WHERE {conditionals.GetConditional()})
RETURNING id
            ";

            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(_insertValueCollector.ParametersValues);
            queryParams.AddDynamicParams(_upsertValueCollector.ParametersValues);
            queryParams.AddDynamicParams(conditionals.GetParameters());

            return new QueryObject(sql, queryParams);
        }
    }
}
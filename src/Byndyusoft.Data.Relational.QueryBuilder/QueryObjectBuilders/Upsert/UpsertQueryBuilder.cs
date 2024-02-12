using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert
{
    public class UpsertQueryBuilder<T>
    {
        private readonly ColumnConverter _columnConverter;
        private readonly T _entity;
        private readonly string _tableName;
        private readonly ValueCollector _valueCollector;
        private readonly List<string> _conflictColumnNames = new();

        private UpsertQueryBuilder(T entity, string tableName, bool isPostgres)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity = entity;

            _tableName = tableName;
            _columnConverter = new ColumnConverter(isPostgres);
            _valueCollector = new ValueCollector(_columnConverter);
        }

        public static UpsertQueryBuilder<T> For(T item, string tableName, bool isPostgres = true)
        {
            return new UpsertQueryBuilder<T>(item, tableName, isPostgres);
        }

        public UpsertQueryBuilder<T> AllPublicValues(
            Func<IEnumerable<TypePropertyInfo>, IEnumerable<TypePropertyInfo>>? transformer = null)
        {
            var properties = _entity.GetPublicPropertyInfos();
            if (transformer != null)
                properties = transformer(properties);
            AddProperties(properties);

            return this;
        }

        private void AddProperties(IEnumerable<TypePropertyInfo> properties)
        {
            foreach (var publicPropertyInfo in properties)
                AddProperty(publicPropertyInfo);
        }

        private void AddProperty(TypePropertyInfo property)
        {
            _valueCollector.Add(_columnConverter.ToColumnName(property.Name), property.Value);
        }

        public UpsertQueryBuilder<T> WithConflictingColumns(params string[] columnNames)
        {
            _conflictColumnNames.AddRange(columnNames);
            return this;
        }

        public UpsertQueryBuilder<T> WithConflictingProperties(params Expression<Func<T, object>>[] properties)
        {
            return WithConflictingColumns(properties
                .Select(propertyExpression => _columnConverter.ToColumnName(propertyExpression))
                .ToArray());
        }

        public QueryObject Build()
        {
            if (_conflictColumnNames.Any() == false)
                throw new InvalidOperationException("Conflict column names are not set");

            var sql = new StringBuilder();

            var fieldParameters = _valueCollector.FieldParameters;
            var insertIntoStatement =
                $"INSERT INTO {_tableName}({string.Join(", ", fieldParameters.Select(x => x.Field))})";
            sql.Append(insertIntoStatement);
            var insertValues = $"VALUES ({string.Join(", ", fieldParameters.Select(x => x.ParamName))})";
            sql.Append(insertValues);
            var columnNames = string.Join(", ", _conflictColumnNames);
            sql.Append($" ON CONFLICT({columnNames}) DO UPDATE SET ");

            var updateElements = fieldParameters
                .Where(x => _conflictColumnNames.Contains(x.Field) == false)
                .Select(x => $"{x.Field} = EXCLUDED.{x.Field} ")
                .ToArray();
            sql.Append(string.Join(", ", updateElements));

            sql.Append("RETURNING id");

            return new QueryObject(sql.ToString(), _valueCollector.ParametersValues);
        }
    }
}
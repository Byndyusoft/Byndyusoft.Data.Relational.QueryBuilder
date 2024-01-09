using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert
{
    public class UpsertQueryBuilder<T> where T : IEntity
    {
        private readonly ColumnConverter _columnConverter;
        private readonly T _entity;
        private readonly string _tableName;
        private readonly ValueCollector _valueCollector;
        private string[]? _conflictColumnNames;

        private UpsertQueryBuilder(T entity, string tableName, bool isPostgres)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity = entity;

            _tableName = tableName;
            _columnConverter = new ColumnConverter(isPostgres);
            _valueCollector = new ValueCollector(_columnConverter);
        }

        public static UpsertQueryBuilderPropertyPicker<T> For(T item, string tableName, bool isPostgres = true)
        {
            var builder = new UpsertQueryBuilder<T>(item, tableName, isPostgres);
            return new UpsertQueryBuilderPropertyPicker<T>(builder);
        }

        internal void IncludeAllPublicValues(
            Func<IEnumerable<TypePropertyInfo>, IEnumerable<TypePropertyInfo>>? transformer = null)
        {
            var properties = _entity.GetPublicPropertyInfos();
            if (transformer != null)
                properties = transformer(properties);
            AddProperties(properties);
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

        internal void SetConflictingProperties<TProp>(params Expression<Func<T, TProp>>[] property)
        {
            _conflictColumnNames = property
                .Select(propertyExpression => _columnConverter.ToColumnName(propertyExpression)).ToArray();
        }

        public QueryObject Build()
        {
            if (_conflictColumnNames == null)
                throw new InvalidOperationException("Conflict column names are not set");

            var sql = new StringBuilder();

            var fieldParameters = _valueCollector.FieldParameters;
            var insertIntoStatement =
                $"INSERT INTO {_tableName}({string.Join(", ", fieldParameters.Select(x => x.Field))})";
            sql.Append(insertIntoStatement);
            var insertValues = $"VALUES ({string.Join(", ", fieldParameters.Select(x => x.ParamName))})";
            sql.Append(insertValues);
            var columnNames = string.Join(", ", _conflictColumnNames);
            sql.Append($" ON CONFLICT({columnNames}) DO Update SET ");

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
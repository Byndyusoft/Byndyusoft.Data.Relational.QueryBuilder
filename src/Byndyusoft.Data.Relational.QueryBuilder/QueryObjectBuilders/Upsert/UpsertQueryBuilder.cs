using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert
{
    public class UpsertQueryBuilder<T> where T : IEntity
    {
        private readonly ColumnConverter _columnConverter;
        private readonly T _entity;
        private readonly string _tableName;
        private readonly ValueCollector _valueCollector;
        private string _conflictColumnName = string.Empty;

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

        internal void IncludeAllPublicValues(Func<IEnumerable<TypePropertyInfo>, IEnumerable<TypePropertyInfo>>? transformer = null)
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

        internal void SetConflictingProperty<TProp>(Expression<Func<T, TProp>> property)
        {
            _conflictColumnName = _columnConverter.ToColumnName(property);
        }

        public QueryObject Build()
        {
            var sql = new StringBuilder();

            var fieldParameters = _valueCollector.FieldParameters;
            var insertIntoStatement = $"INSERT INTO {_tableName}({string.Join(", ", fieldParameters.Select(x => x.Field))})";
            sql.Append(insertIntoStatement);
            var insertValues = $"VALUES ({string.Join(", ", fieldParameters.Select(x => x.ParamName))})";
            sql.Append(insertValues);
            sql.Append($" ON CONFLICT({_conflictColumnName}) DO Update SET ");

            var updateElements = fieldParameters
                .Where(x => x.Field != _conflictColumnName)
                .Select(x => $"{x.Field} = EXCLUDED.{x.Field} ")
                .ToArray();
            sql.Append(string.Join(", ", updateElements));

            sql.Append("RETURNING id");

            return new QueryObject(sql.ToString(), _valueCollector.ParametersValues);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;
using Dapper;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public class InsertQueryBuilder
    {
        private readonly string _tableName;
        public readonly ColumnConverter ColumnConverter;
        protected readonly ValueCollector ValueCollector;
        private string? _withConflictDoNothing;
        private bool _withReturningId = true;

        protected InsertQueryBuilder(string tableName, bool isPostgres = true)
        {
            _tableName = tableName;
            ColumnConverter = new ColumnConverter(isPostgres);
            ValueCollector = new ValueCollector(ColumnConverter);
        }

        public static InsertQueryBuilder For(string tableName)
        {
            return new InsertQueryBuilder(tableName);
        }

        public InsertQueryBuilder ValueExpression(string fieldName, string value, Func<string, string>? paramValueTransformer = null)
        {
            ValueCollector.AddExpression(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder Value(string fieldName, object? value, Func<string, string>? paramValueTransformer = null)
        {
            ValueCollector.Add(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder Value(TypePropertyInfo propertyInfo, Func<string, string>? paramValueTransformer = null)
        {
            ValueCollector.Add(ColumnConverter.ToColumnName(propertyInfo.Name), propertyInfo.Value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder ValueExpression<T, TProp>(
            Expression<Func<T, TProp>> action, 
            string value, Func<string, string>? paramValueTransformer = null)
        {
            string fieldName = ColumnConverter.ToColumnName(action);

            ValueExpression(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder Value<T, TProp>(
            T item, 
            Expression<Func<T, TProp>> action, 
            Func<string, string>? paramValueTransformer = null)
        {
            string fieldName = ColumnConverter.ToColumnName(action);
            var value = ExpressionsCache<T, TProp>.Get(action)(item);

            Value(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder Value<T, TProp>(Expression<Func<T, TProp>> action, TProp value, Func<string, string>? paramValueTransformer = null)
        {
            string fieldName = ColumnConverter.ToColumnName(action);

            Value(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder CustomValue<T, TProp>(
            Expression<Func<T, TProp>> action, 
            object? value, 
            Func<string, string>? paramValueTransformer = null)
        {
            string fieldName = ColumnConverter.ToColumnName(action);

            Value(fieldName, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder WithConflictDoNothing(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));

            if (string.IsNullOrEmpty(_withConflictDoNothing) == false)
                throw new Exception("WithConflictDoNothing already added");

            _withConflictDoNothing = columnName;
            return this;
        }

        public InsertQueryBuilder WithConflictDoNothing<T, TProp>(Expression<Func<T, TProp>> property)
        {
            return WithConflictDoNothing(ColumnConverter.ToColumnName(property));
        }

        public InsertQueryBuilder WithoutReturningId()
        {
            _withReturningId = false;
            return this;
        }

        public QueryObject Build()
        {
            var sql = GetSql();

            return new QueryObject(sql, GetParameters());
        }

        public string GetSql()
        {
            return $@"
{GetInsertIntoStatement()}
{GetValuesStatement()}
{GetAfterInsertStatement()}";
        }

        public string GetInsertIntoStatement()
        {
            var fieldParameters = ValueCollector.FieldParameters;

            return $"INSERT INTO {_tableName}({string.Join(", ", fieldParameters.Select(x => x.Field))})";
        }

        public string GetValuesStatement()
        {
            var fieldParameters = ValueCollector.FieldParameters;

            return $"VALUES ({string.Join(", ", fieldParameters.Select(x => x.ParamName))})";
        }

        public string GetAfterInsertStatement()
        {
            var statement = "";
            if (string.IsNullOrEmpty(_withConflictDoNothing) == false)
                statement += $"ON CONFLICT({_withConflictDoNothing}) DO NOTHING ";
            if (_withReturningId)
                statement += "RETURNING id";

            return statement;
        }

        public DynamicParameters GetParameters()
        {
            return ValueCollector.ParametersValues;
        }

        public static InsertQueryBuilder<T> For<T>(T item, string tableName, bool isPostgres = true)
        {
            return InsertQueryBuilder<T>.For(item, tableName, isPostgres);
        }
    }

    public class InsertQueryBuilder<T> : InsertQueryBuilder
    {
        protected InsertQueryBuilder(T entity, string tableName, bool isPostgres) : base(tableName, isPostgres)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entity = entity;
        }

        public T Entity { get; }

        public static InsertQueryBuilder<T> For(T item, string tableName, bool isPostgres = true)
        {
            return new InsertQueryBuilder<T>(item, tableName, isPostgres);
        }

        public InsertQueryBuilder<T> ValueExpression<TProp>(
            Expression<Func<T, TProp>> action, 
            string value, 
            Func<string, string>? paramValueTransformer = null)
        {
            base.ValueExpression(action, value, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder<T> Value<TProp>(
            Expression<Func<T, TProp>> action, 
            Func<string, string>? paramValueTransformer = null)
        {
            Value(Entity, action, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder CustomValue<TProp>(
            Expression<Func<T, TProp>> action, 
            object? customValue, 
            Func<string, string>? paramValueTransformer = null)
        {
            CustomValue<T, TProp>(action, customValue, paramValueTransformer);
            return this;
        }

        public InsertQueryBuilder<T> Values(IEnumerable<TypePropertyInfo> properties)
        {
            foreach (var publicPropertyInfo in properties) Value(publicPropertyInfo);

            return this;
        }

        public InsertQueryBuilder WithConflictDoNothing<TProp>(Expression<Func<T, TProp>> property)
        {
            return base.WithConflictDoNothing(property);
        }

        public InsertQueryBuilder<T> AllPublicValues(
            Func<IEnumerable<TypePropertyInfo<T>>, IEnumerable<TypePropertyInfo<T>>>? transformer = null)
        {
            var properties = Entity.GetPublicPropertyInfos();
            if (transformer != null)
                properties = transformer(properties);
            return Values(properties);
        }

        public InsertQueryBuilder<T> If(bool condition, Action<InsertQueryBuilder<T>> thenStatement,
            Action<InsertQueryBuilder<T>>? elseStatement = null)
        {
            if (condition)
                thenStatement(this);
            else
                elseStatement?.Invoke(this);

            return this;
        }
    }
}
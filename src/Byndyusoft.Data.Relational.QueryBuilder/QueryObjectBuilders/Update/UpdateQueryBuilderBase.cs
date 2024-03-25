using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;
using Dapper;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update
{
    public abstract class UpdateQueryBuilderBase<TBuilder>
        where TBuilder : UpdateQueryBuilderBase<TBuilder>
    {
        private readonly string _tableName;
        protected readonly ColumnConverter ColumnConverter;
        protected readonly ConditionalCollector Conditionals;
        protected readonly List<string> Expressions = new List<string>();
        protected readonly string? TableAlias;
        protected readonly ValueCollector ValueCollector;
        private string? _from;
        private object? _fromParametersObj;

        protected UpdateQueryBuilderBase(string tableName, string? tableAlias, bool isPostgres)
        {
            _tableName = tableName;
            TableAlias = tableAlias;
            ColumnConverter = new ColumnConverter(isPostgres);
            Conditionals = new ConditionalCollector(ColumnConverter);
            ValueCollector = new ValueCollector(ColumnConverter);
        }

        public TBuilder Where(FormattableString conditional)
        {
            Conditionals.AddFormattableString(conditional);
            return (TBuilder)this;
        }

        public TBuilder SetExpression(string expression)
        {
            Expressions.Add(expression);
            return (TBuilder)this;
        }

        public TBuilder Set(string fieldName, object? value, Func<string, string>? paramValueTransformer = null)
        {
            ValueCollector.Add(fieldName, value, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder Set(TypePropertyInfo propertyInfo, Func<string, string>? paramValueTransformer = null)
        {
            ValueCollector.Add(ColumnConverter.ToColumnName(propertyInfo.Name), propertyInfo.Value, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder Set<T, TProp>(Expression<Func<T, TProp>> action, TProp value, Func<string, string>? paramValueTransformer = null)
        {
            var fieldName = ColumnConverter.ToColumnName(action);

            Set(fieldName, value, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder SetCustomValue<T, TProp>(
            Expression<Func<T, TProp>> action, 
            object? customValue, 
            Func<string, string>? paramValueTransformer = null)
        {
            var fieldName = ColumnConverter.ToColumnName(action);

            Set(fieldName, customValue, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder From<T>(Expression<Func<T, string>> formatString, string? table2Alias = null,
            object? parametersObj = null)
        {
            _from = QueryBuilder<T>.For(ColumnConverter).Get(formatString, table2Alias);
            _fromParametersObj = parametersObj;
            return (TBuilder)this;
        }

        public QueryObject Build()
        {
            var fields = ValueCollector.FieldParameters.Select(x => $"{x.Field} = {x.ParamName}").Concat(Expressions);

            var sql = $@"
UPDATE {_tableName} {TableAlias} SET {string.Join(", ", fields)}";

            if (string.IsNullOrEmpty(_from) == false)
                sql += $@"
FROM {_from}";

            sql += $@"
WHERE {Conditionals.GetConditional()}";

            var values = new DynamicParameters();
            values.AddDynamicParams(ValueCollector.ParametersValues);
            values.AddDynamicParams(Conditionals.GetParameters());
            if (_fromParametersObj != null)
                values.AddDynamicParams(_fromParametersObj);

            return new QueryObject(sql, values);
        }
    }

    public abstract class UpdateQueryBuilderBase<TBuilder, T> : UpdateQueryBuilderBase<TBuilder>
        where TBuilder : UpdateQueryBuilderBase<TBuilder, T>
    {
        protected UpdateQueryBuilderBase(string tableName, string? tableAlias, bool isPostgres) : base(tableName,
            tableAlias, isPostgres)
        {
        }

        public TBuilder Set<TProp>(Expression<Func<T, TProp>> action, TProp value, Func<string, string>? paramValueTransformer = null)
        {
            Set<T, TProp>(action, value, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder SetCustomValue<TProp>(
            Expression<Func<T, TProp>> action, 
            object? customValue, 
            Func<string, string>? paramValueTransformer = null)
        {
            SetCustomValue<T, TProp>(action, customValue, paramValueTransformer);
            return (TBuilder)this;
        }

        public TBuilder SetExpression(Expression<Func<T, string>> action)
        {
            var expression = ColumnConverter.Map(action);
            SetExpression(expression);
            return (TBuilder)this;
        }

        public TBuilder SetExpression<T2>(Expression<Func<T, T2, string>> action, string? table2Alias = null)
        {
            var expression = ColumnConverter.Map(action, null, table2Alias);
            SetExpression(expression);
            return (TBuilder)this;
        }

        public ConditionalCollectorWrapper<T> GetConditionals()
        {
            return Conditionals.For<T>(TableAlias);
        }

        public TBuilder WhereEquals<TProp>(Expression<Func<T, TProp>> action, TProp value)
        {
            Conditionals.AddEquals(action, value, TableAlias);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNull<TProp>(Expression<Func<T, TProp>> action)
        {
            Conditionals.AddNull(action, TableAlias);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNotNull<TProp>(Expression<Func<T, TProp>> action)
        {
            Conditionals.AddNotNull(action, TableAlias);
            return (TBuilder)this;
        }

        public TBuilder WhereExpression<T2>(Expression<Func<T, T2, string>> action, string? table2Alias = null)
        {
            var expression = ColumnConverter.Map(action, TableAlias, table2Alias);
            Conditionals.Add(expression, null);
            return (TBuilder)this;
        }

        public TBuilder If(bool condition, Func<UpdateQueryBuilderBase<TBuilder, T>, TBuilder> ifAction,
            Func<UpdateQueryBuilderBase<TBuilder, T>, TBuilder>? elseAction = null)
        {
            if (ifAction == null)
                throw new ArgumentNullException(nameof(ifAction));

            if (condition)
                return ifAction(this);

            return elseAction?.Invoke(this) ?? (TBuilder)this;
        }
    }
}
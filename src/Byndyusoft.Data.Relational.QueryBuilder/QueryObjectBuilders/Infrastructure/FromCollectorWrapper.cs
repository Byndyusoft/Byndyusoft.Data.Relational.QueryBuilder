using System;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class FromCollectorWrapper<T>
    {
        private readonly FromCollector _fromCollector;
        private readonly string? _tableAlias;

        private FromCollectorWrapper(FromCollector fromCollector, string? tableAlias)
        {
            _fromCollector = fromCollector;
            _tableAlias = tableAlias;
        }

        public static FromCollectorWrapper<T> For(FromCollector fromCollector, string? tableAlias)
        {
            return new FromCollectorWrapper<T>(fromCollector, tableAlias);
        }

        public FromCollectorWrapper<T> From<TTo>(string tableToAlias, Expression<Func<T, TTo, string>> formatString)
        {
            var expression = _fromCollector.ColumnConverter.Map(formatString, _tableAlias, tableToAlias);
            _fromCollector.AddExpression(expression);

            return this;
        }

        public FromCollectorWrapper<T> LeftJoin<TTo>(string tableToName, string tableToAlias,
            Expression<Func<T, TTo, string>> formatString)
        {
            _fromCollector.AddLeftJoin(tableToName, tableToAlias,
                _fromCollector.ColumnConverter.Map(formatString, _tableAlias, tableToAlias));

            return this;
        }

        public FromCollectorWrapper<T> InnerJoin<TTo>(string tableToName, string tableToAlias,
            Expression<Func<T, TTo, string>> formatString)
        {
            _fromCollector.AddInnerJoin(tableToName, tableToAlias,
                _fromCollector.ColumnConverter.Map(formatString, _tableAlias, tableToAlias));

            return this;
        }

        public FromCollectorWrapper<T> InnerJoin(string tableToName, string tableToAlias,
            Expression<Func<T, long>> property)
        {
            _fromCollector.AddInnerJoin<T, IEntity, long>(tableToName, _tableAlias, tableToAlias, property, i => i.Id);
            return this;
        }

        public FromCollectorWrapper<T> LeftJoin(string tableToName, string tableToAlias,
            Expression<Func<T, long?>> property)
        {
            _fromCollector.AddLeftJoin<T, IEntity, long?>(tableToName, _tableAlias, tableToAlias, property, i => i.Id);
            return this;
        }

        public FromCollectorWrapper<T> InnerJoin(string tableToName, string tableToAlias,
            Expression<Func<T, long?>> property)
        {
            _fromCollector.AddInnerJoin<T, IEntity, long?>(tableToName, _tableAlias, tableToAlias, property, i => i.Id);
            return this;
        }

        public FromCollectorWrapper<T> AddExpression<T2>(Expression<Func<T2, string>> expression,
            string? tableAlias = null)
        {
            _fromCollector.AddExpression(expression, tableAlias);
            return this;
        }

        public FromCollectorWrapper<T> AddExpression<T2>(Expression<Func<T, T2, string>> expression,
            string? table2Alias = null)
        {
            _fromCollector.AddExpression(expression, _tableAlias, table2Alias);
            return this;
        }

        public FromCollectorWrapper<T> AddExpression<T2, T3>(Expression<Func<T, T2, T3, string>> expression,
            string? table2Alias = null, string? table3Alias = null)
        {
            _fromCollector.AddExpression(expression, _tableAlias, table2Alias, table3Alias);
            return this;
        }

        public FromCollectorWrapper<TOther> Other<TOther>(string? tableAlias = null)
        {
            return FromCollectorWrapper<TOther>.For(_fromCollector, tableAlias);
        }

        public FromCollectorWrapper<T, TProp> With<TProp>(Expression<Func<T, TProp>> property)
        {
            return FromCollectorWrapper<T, TProp>.For(_fromCollector, _tableAlias, property, this);
        }
    }

    public class FromCollectorWrapper<T, TProp>
    {
        private readonly FromCollector _fromCollector;
        private readonly FromCollectorWrapper<T> _fromCollectorWrapper;
        private readonly Expression<Func<T, TProp>> _property;
        private readonly string? _tableAlias;

        private FromCollectorWrapper(FromCollector fromCollector, string? tableAlias,
            Expression<Func<T, TProp>> property, FromCollectorWrapper<T> fromCollectorWrapper)
        {
            _fromCollector = fromCollector;
            _tableAlias = tableAlias;
            _property = property;
            _fromCollectorWrapper = fromCollectorWrapper;
        }

        public static FromCollectorWrapper<T, TProp> For(FromCollector fromCollector, string? tableAlias,
            Expression<Func<T, TProp>> property, FromCollectorWrapper<T> fromCollectorWrapper)
        {
            return new FromCollectorWrapper<T, TProp>(fromCollector, tableAlias, property, fromCollectorWrapper);
        }

        public FromCollectorWrapper<T> LeftJoin<TTo>(string tableToName, string tableToAlias,
            Expression<Func<TTo, TProp>> toProperty)
        {
            _fromCollector.AddLeftJoin(tableToName, _tableAlias, tableToAlias, _property, toProperty);
            return _fromCollectorWrapper;
        }

        public FromCollectorWrapper<T> InnerJoin<TTo>(string tableToName, string tableToAlias,
            Expression<Func<TTo, TProp>> toProperty)
        {
            _fromCollector.AddInnerJoin(tableToName, _tableAlias, tableToAlias, _property, toProperty);
            return _fromCollectorWrapper;
        }
    }
}
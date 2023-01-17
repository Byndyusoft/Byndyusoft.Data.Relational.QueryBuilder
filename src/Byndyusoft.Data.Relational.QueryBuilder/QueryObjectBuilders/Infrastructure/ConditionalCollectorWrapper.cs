using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class ConditionalCollectorWrapper<T>
    {
        private readonly ConditionalCollector _conditionalCollector;

        public ConditionalCollectorWrapper(ConditionalCollector conditionalCollector, string? tableAlias)
        {
            _conditionalCollector = conditionalCollector;
            TableAlias = tableAlias;
        }

        public string? TableAlias { get; }
        public ColumnConverter ColumnConverter => _conditionalCollector.ColumnConverter;

        public ConditionalCollectorWrapper<T> Add(Expression<Func<T, string>> formatString, object? template = null)
        {
            _conditionalCollector.AddExpression(formatString, template, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> Add<T2>(Expression<Func<T, T2, string>> formatString,
            object? template = null, string? table2Alias = null)
        {
            _conditionalCollector.AddExpression(formatString, template, TableAlias, table2Alias);
            return this;
        }

        public ConditionalCollectorWrapper<T> Add<T2, T3>(Expression<Func<T, T2, T3, string>> formatString,
            object? template = null, string? table2Alias = null, string? table3Alias = null)
        {
            _conditionalCollector.AddExpression(formatString, template, TableAlias, table2Alias, table3Alias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddNull<TProp>(Expression<Func<T, TProp>> func)
        {
            _conditionalCollector.AddNull(func, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddNotNull<TProp>(Expression<Func<T, TProp>> func)
        {
            _conditionalCollector.AddNotNull(func, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddEquals(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddEquals<TProp>(Expression<Func<T, TProp?>> property, TProp value)
            where TProp : struct
        {
            _conditionalCollector.AddEquals(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddNotEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddNotEquals(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddIn<TProp>(Expression<Func<T, TProp>> property, IReadOnlyCollection<TProp> values)
        {
            _conditionalCollector.AddIn(property, values, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddIn<TProp>(Expression<Func<T, TProp?>> property, IReadOnlyCollection<TProp> values)
            where TProp : struct
        {
            _conditionalCollector.AddIn(property, values, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddNotIn<TProp>(Expression<Func<T, TProp>> property, IReadOnlyCollection<TProp> values)
        {
            _conditionalCollector.AddNotIn(property, values, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddNotIn<TProp>(Expression<Func<T, TProp?>> property, IReadOnlyCollection<TProp> values)
            where TProp : struct
        {
            _conditionalCollector.AddNotIn(property, values, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddGreater<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddGreater(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddGreaterOrEqual<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddGreaterOrEqual(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddLess<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddLess(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddLessOrEqual<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            _conditionalCollector.AddLessOrEqual(property, value, TableAlias);
            return this;
        }

        public ConditionalCollectorWrapper<T> AddOr(params Action<ConditionalCollectorWrapper<T>>[] orConditions)
        {
            if (orConditions == null)
                throw new ArgumentNullException(nameof(orConditions));

            var conditionalCollectorWrappers = orConditions.Select(AddOrSingle).ToArray();
            _conditionalCollector.AddOr(conditionalCollectorWrappers);

            return this;
        }

        private ConditionalCollector AddOrSingle(Action<ConditionalCollectorWrapper<T>> orCondition)
        {
            var wrapper = _conditionalCollector.GetNextOrConditionalCollector().For<T>(TableAlias);
            orCondition(wrapper);
            return wrapper._conditionalCollector;
        }
    }
}
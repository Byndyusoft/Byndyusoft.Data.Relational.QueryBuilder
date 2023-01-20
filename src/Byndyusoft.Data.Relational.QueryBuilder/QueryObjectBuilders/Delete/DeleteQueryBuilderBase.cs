using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Delete
{
    public abstract class DeleteQueryBuilderBase<TBuilder>
        where TBuilder : DeleteQueryBuilderBase<TBuilder>
    {
        private readonly string _tableName;
        protected readonly ConditionalCollector Conditionals;
        private bool _allRows;

        protected DeleteQueryBuilderBase(string tableName, bool isPostgres)
        {
            _tableName = tableName;
            Conditionals = new ConditionalCollector(new ColumnConverter(isPostgres));
        }

        public virtual QueryObject Build()
        {
            var sql = $@"
DELETE FROM {_tableName}";

            if (_allRows == false)
                sql += $@"
WHERE {Conditionals.GetConditional()}";

            return new QueryObject(sql, Conditionals.GetParameters());
        }

        public TBuilder Where(FormattableString condition)
        {
            Conditionals.AddFormattableString(condition);
            return (TBuilder)this;
        }

        public TBuilder AllRows()
        {
            _allRows = true;
            return (TBuilder)this;
        }
    }

    public abstract class DeleteQueryBuilderBase<TBuilder, T> : DeleteQueryBuilderBase<TBuilder>
        where TBuilder : DeleteQueryBuilderBase<TBuilder, T>
    {
        protected DeleteQueryBuilderBase(string tableName, bool isPostgres)
            : base(tableName, isPostgres)
        {
        }

        public ConditionalCollectorWrapper<T> GetConditionals()
        {
            return Conditionals.For<T>();
        }

        public TBuilder WhereEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            Conditionals.AddEquals(property, value);
            return (TBuilder)this;
        }

        public TBuilder WhereEquals<TProp>(Expression<Func<T, TProp?>> property, TProp value)
            where TProp : struct
        {
            Conditionals.AddEquals(property, value);
            return (TBuilder)this;
        }

        public TBuilder WhereNotEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            Conditionals.AddNotEquals(property, value);
            return (TBuilder)this;
        }

        public TBuilder WhereNotEquals<TProp>(Expression<Func<T, TProp?>> property, TProp value)
            where TProp : struct
        {
            Conditionals.AddNotEquals(property, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNull<TProp>(Expression<Func<T, TProp>> action)
        {
            Conditionals.AddNull(action);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNotNull<TProp>(Expression<Func<T, TProp>> action)
        {
            Conditionals.AddNotNull(action);
            return (TBuilder)this;
        }

        public TBuilder WhereIsIn<TProp>(Expression<Func<T, TProp>> action, IReadOnlyCollection<TProp> values)
        {
            Conditionals.AddIn(action, values);
            return (TBuilder)this;
        }

        public TBuilder WhereIsIn<TProp>(Expression<Func<T, TProp?>> action, IReadOnlyCollection<TProp> values)
            where TProp : struct
        {
            Conditionals.AddIn(action, values);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNotIn<TProp>(Expression<Func<T, TProp>> action, IReadOnlyCollection<TProp> values)
        {
            Conditionals.AddNotIn(action, values);
            return (TBuilder)this;
        }

        public TBuilder WhereIsNotIn<TProp>(Expression<Func<T, TProp?>> action, IReadOnlyCollection<TProp> values)
            where TProp : struct
        {
            Conditionals.AddNotIn(action, values);
            return (TBuilder)this;
        }

        public TBuilder WhereIsLess<TProp>(Expression<Func<T, TProp>> action, TProp value)
        {
            Conditionals.AddLess(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsLess<TProp>(Expression<Func<T, TProp?>> action, TProp value)
            where TProp : struct
        {
            Conditionals.AddLess(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsLessOrEqual<TProp>(Expression<Func<T, TProp>> action, TProp value)
        {
            Conditionals.AddLessOrEqual(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsLessOrEqual<TProp>(Expression<Func<T, TProp?>> action, TProp value)
            where TProp : struct
        {
            Conditionals.AddLessOrEqual(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsGreater<TProp>(Expression<Func<T, TProp>> action, TProp value)
        {
            Conditionals.AddGreater(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsGreater<TProp>(Expression<Func<T, TProp?>> action, TProp value)
            where TProp : struct
        {
            Conditionals.AddGreater(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsGreaterOrEqual<TProp>(Expression<Func<T, TProp>> action, TProp value)
        {
            Conditionals.AddGreaterOrEqual(action, value);
            return (TBuilder)this;
        }

        public TBuilder WhereIsGreaterOrEqual<TProp>(Expression<Func<T, TProp?>> action, TProp value)
            where TProp : struct
        {
            Conditionals.AddGreaterOrEqual(action, value);
            return (TBuilder)this;
        }
    }
}
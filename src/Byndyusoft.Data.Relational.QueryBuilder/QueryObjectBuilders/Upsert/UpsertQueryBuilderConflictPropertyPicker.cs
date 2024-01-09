using System;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert
{
    public class UpsertQueryBuilderConflictPropertyPicker<T> where T : IEntity
    {
        private readonly UpsertQueryBuilder<T> _builder;

        public UpsertQueryBuilderConflictPropertyPicker(UpsertQueryBuilder<T> builder)
        {
            _builder = builder;
        }

        public UpsertQueryBuilder<T> WithConflictingProperty<TProp>(Expression<Func<T, TProp>> property)
        {
            _builder.SetConflictingProperties(property);
            return _builder;
        }

        public UpsertQueryBuilder<T> WithConflictingProperties(params Expression<Func<T, object>>[] properties)
        {
            _builder.SetConflictingProperties(properties);
            return _builder;
        }
    }
}
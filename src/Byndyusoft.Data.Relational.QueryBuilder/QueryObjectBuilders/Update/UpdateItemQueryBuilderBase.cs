using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update
{
    public abstract class UpdateItemQueryBuilderBase<TBuilder, T> : UpdateQueryBuilderBase<TBuilder, T>
        where TBuilder : UpdateItemQueryBuilderBase<TBuilder, T>
    {
        protected UpdateItemQueryBuilderBase(T entity, string tableName, string? tableAlias, bool isPostgres)
            : base(tableName, tableAlias, isPostgres)
        {
            Entity = entity;
        }

        public T Entity { get; }

        public TBuilder Set<TProp>(Expression<Func<T, TProp>> action)
        {
            var fieldName = ColumnConverter.ToColumnName(action);
            var value = ExpressionsCache<T, TProp>.Get(action)(Entity);

            Set(fieldName, value);
            return (TBuilder)this;
        }

        public TBuilder Set(IEnumerable<TypePropertyInfo> properties)
        {
            foreach (var publicPropertyInfo in properties) Set(publicPropertyInfo);

            return (TBuilder)this;
        }

        public TBuilder AllPublicValues(Func<IEnumerable<TypePropertyInfo>, IEnumerable<TypePropertyInfo>>? transformer = null)
        {
            var properties = Entity.GetPublicPropertyInfos();
            if (transformer != null)
                properties = transformer(properties);
            Set(properties);
            return (TBuilder)this;
        }
    }
}
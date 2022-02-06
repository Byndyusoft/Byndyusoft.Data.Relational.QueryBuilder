using System;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class GroupByCollectorWrapper<T>
    {
        private readonly GroupByCollector _groupByCollector;
        private readonly string? _tableAlias;

        public GroupByCollectorWrapper(GroupByCollector groupByCollector, string? tableAlias)
        {
            _groupByCollector = groupByCollector;
            _tableAlias = tableAlias;
        }

        public GroupByCollectorWrapper<T> Add<TProp>(Expression<Func<T, TProp>> property)
        {
            _groupByCollector.Add(property, _tableAlias);
            return this;
        }
    }
}
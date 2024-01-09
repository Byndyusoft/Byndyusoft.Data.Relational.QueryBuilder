using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public static class ExpressionsCache<TIn, TResult>
    {
        private static readonly ConcurrentDictionary<string, Func<TIn, TResult>> CachedFunc
            = new ConcurrentDictionary<string, Func<TIn, TResult>>();

        public static Func<TIn, TResult> Get(Expression<Func<TIn, TResult>> action)
        {
            var key = action.GetPropertyName();
            return CachedFunc.GetOrAdd(key, x => action.Compile());
        }
    }
}
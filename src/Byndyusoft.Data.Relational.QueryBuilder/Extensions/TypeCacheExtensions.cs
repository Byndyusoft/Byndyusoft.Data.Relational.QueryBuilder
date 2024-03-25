using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Extensions
{
    public static class TypeCacheExtensions
    {
        public static IEnumerable<TypePropertyInfo<T>> GetPublicPropertyInfos<T>(this T value)
        {
            return TypeCache<T>.GetPublicPropertyInfos(value);
        }

        public static IEnumerable<TypePropertyInfo<T>> Exclude<T>(this IEnumerable<TypePropertyInfo<T>> infos,
            Expression<Func<T, object?>> property)
        {
            var propertyName = property.GetPropertyName();
            return infos.Where(i => i.Name != propertyName);
        }

        public static IEnumerable<string> Exclude<T, TProp>(this IEnumerable<string> infos,
            Expression<Func<T, TProp>> property)
        {
            var propertyName = property.GetPropertyName();
            return infos.Where(i => i != propertyName);
        }
    }
}
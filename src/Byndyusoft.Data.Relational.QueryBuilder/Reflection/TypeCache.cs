using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Reflection
{
    public class TypeCache<T>
    {
        private static readonly Lazy<TypeCache<T>> Lazy = new Lazy<TypeCache<T>>(() => new TypeCache<T>());

        private readonly PropertyInfo[] _publicProperties;

        private TypeCache()
        {
            _publicProperties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.GetMethod != null && i.SetMethod != null).ToArray();
        }

        private static TypeCache<T> Instance => Lazy.Value;

        public static IEnumerable<TypePropertyInfo> GetPublicPropertyInfos(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Instance._publicProperties.Select(i => new TypePropertyInfo(i.Name, i.GetValue(value)));
        }

        public static IEnumerable<string> GetPublicPropertyNames()
        {
            return Instance._publicProperties.Select(i => i.Name);
        }

        public static IEnumerable<PropertyInfo> GetPublicProperties()
        {
            return Instance._publicProperties;
        }
    }
}
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using System;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class ColumnConverter
    {
        public ColumnConverter(bool isPostgres)
        {
            IsPostgres = isPostgres;
        }

        public bool IsPostgres { get; }

        public string ToColumnName(string propertyName, string? tableAlias = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (IsPostgres)
                propertyName = SnakeCase.Get(propertyName);

            if (string.IsNullOrEmpty(tableAlias) == false)
                propertyName = $"{tableAlias}.{propertyName}";

            return propertyName;
        }

        public string ToColumnName<T, TProp>(Expression<Func<T, TProp>> property, string? tableAlias = null)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return ToColumnName(property.GetPropertyName(), tableAlias);
        }

        public object? ToArgument(object? value)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            if (type.IsEnum && Enum.GetUnderlyingType(type) == typeof(int))
                return (int)value;

            return value;
        }
    }
}
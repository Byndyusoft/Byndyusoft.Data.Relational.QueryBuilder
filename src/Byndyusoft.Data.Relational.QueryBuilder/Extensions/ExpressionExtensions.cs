using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetPropertyName<T, TProp>(this Expression<Func<T, TProp>> expression)
        {
            var property = GetProperty(expression);
            return property.Name;
        }

        private static PropertyInfo GetProperty<T, TProp>(Expression<Func<T, TProp>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body;
            if (memberExpression is UnaryExpression unaryExpression &&
                unaryExpression.NodeType == ExpressionType.Convert)
                memberExpression = unaryExpression.Operand;

            var body = (MemberExpression)memberExpression;
            var property = body.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException($"Member {body.Member.Name} is not a property");

            return property;
        }

        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        public static void SetPropertyValue<T, TProp>(this Expression<Func<T, TProp>> expression, T item, TProp value)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var property = GetProperty(expression);
            if (property.PropertyType != typeof(TProp))
                throw new ArgumentException(
                    $"Тип возвращаемого значения свойства {property.Name} не совпадает с искомым {typeof(TProp).Name}");

            property.SetValue(item, value);
        }

        public static TProp GetPropertyValue<T, TProp>(this Expression<Func<T, TProp>> expression, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var property = GetProperty(expression);
            return (TProp)property.GetValue(item);
        }
    }
}
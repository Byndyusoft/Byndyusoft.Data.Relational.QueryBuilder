using System;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;

namespace Byndyusoft.Data.Relational.QueryBuilder
{
    public static class Sql
    {
        public static string Select<T>(Expression<Func<T, string>> expression, string? alias = null)
        {
            alias ??= typeof(T).Name.ToCamelCase();
            return new ColumnConverter(true).Map(expression, alias);
        }

        public static string Select<T1, T2>(Expression<Func<T1, T2, string>> expression, string? alias1 = null,
            string? alias2 = null)
        {
            alias1 ??= typeof(T1).Name.ToCamelCase();
            alias2 ??= typeof(T2).Name.ToCamelCase();
            return new ColumnConverter(true).Map(expression, alias1, alias2);
        }

        public static string Select<T1, T2, T3>(Expression<Func<T1, T2, T3, string>> expression, string? alias1 = null,
            string? alias2 = null, string? alias3 = null)
        {
            alias1 ??= typeof(T1).Name.ToCamelCase();
            alias2 ??= typeof(T2).Name.ToCamelCase();
            alias3 ??= typeof(T3).Name.ToCamelCase();
            return new ColumnConverter(true).Map(expression, alias1, alias2, alias3);
        }

        public static string Select<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, string>> expression,
            string? alias1 = null,
            string? alias2 = null, string? alias3 = null, string? alias4 = null)
        {
            alias1 ??= typeof(T1).Name.ToCamelCase();
            alias2 ??= typeof(T2).Name.ToCamelCase();
            alias3 ??= typeof(T3).Name.ToCamelCase();
            alias4 ??= typeof(T4).Name.ToCamelCase();
            return new ColumnConverter(true).Map(expression, alias1, alias2, alias3, alias4);
        }


        private static string ToCamelCase(this string input)
        {
            return char.ToLowerInvariant(input[0]) + input[1..];
        }
    }
}
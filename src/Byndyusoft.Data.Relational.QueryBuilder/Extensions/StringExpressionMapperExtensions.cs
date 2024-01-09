using System;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Tools;

namespace Byndyusoft.Data.Relational.QueryBuilder.Extensions
{
    public static class StringExpressionMapperExtensions
    {
        public static string Map<T>(this ColumnConverter columnConverter, Expression<Func<T, string>> formatString,
            string? alias = null)
        {
            return StringExpressionMapper.Map(formatString, s => columnConverter.ToColumnName(s, alias));
        }

        public static string Map<T1, T2>(this ColumnConverter columnConverter,
            Expression<Func<T1, T2, string>> formatString,
            string? alias1, string? alias2)
        {
            return StringExpressionMapper.Map(formatString, s => columnConverter.ToColumnName(s, alias1),
                s => columnConverter.ToColumnName(s, alias2));
        }

        public static string Map<T1, T2, T3>(this ColumnConverter columnConverter,
            Expression<Func<T1, T2, T3, string>> formatString,
            string? alias1, string? alias2, string? alias3)
        {
            return StringExpressionMapper.Map(formatString, s => columnConverter.ToColumnName(s, alias1),
                s => columnConverter.ToColumnName(s, alias2), s => columnConverter.ToColumnName(s, alias3));
        }

        public static string Map<T1, T2, T3, T4>(this ColumnConverter columnConverter,
            Expression<Func<T1, T2, T3, T4, string>> formatString,
            string? alias1, string? alias2, string? alias3, string? alias4)
        {
            return StringExpressionMapper.Map(formatString, s => columnConverter.ToColumnName(s, alias1),
                s => columnConverter.ToColumnName(s, alias2), s => columnConverter.ToColumnName(s, alias3),
                s => columnConverter.ToColumnName(s, alias4));
        }
    }
}
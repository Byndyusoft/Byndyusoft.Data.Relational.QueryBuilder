using System;
using System.Linq.Expressions;
using System.Text;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class FromCollector
    {
        private const string NolockExpr = "WITH(NOLOCK)";
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public FromCollector(ColumnConverter columnConverter)
        {
            ColumnConverter = columnConverter;
        }

        public ColumnConverter ColumnConverter { get; }

        public void AddExpression(string expression)
        {
            if (_stringBuilder.Length != 0)
            {
                _stringBuilder.AppendLine();
                _stringBuilder.Append("	");
            }

            _stringBuilder.Append(expression);
        }

        public void AddExpression<T>(Expression<Func<T, string>> expression, string? tableAlias = null)
        {
            AddExpression(ColumnConverter.Map(expression, tableAlias));
        }

        public void AddExpression<T, T2>(Expression<Func<T, T2, string>> expression, string? tableAlias = null,
            string? table2Alias = null)
        {
            AddExpression(ColumnConverter.Map(expression, tableAlias, table2Alias));
        }

        public void AddExpression<T, T2, T3>(Expression<Func<T, T2, T3, string>> expression, string? tableAlias = null,
            string? table2Alias = null, string? table3Alias = null)
        {
            AddExpression(ColumnConverter.Map(expression, tableAlias, table2Alias, table3Alias));
        }

        public void Add(string tableName, string? tableAlias = null, bool withNoLock = false)
        {
            if (withNoLock)
                AddExpression(string.IsNullOrEmpty(tableAlias)
                    ? $"{tableName} {NolockExpr}"
                    : $"{tableName} AS {tableAlias} {NolockExpr}");
            else
                AddExpression(string.IsNullOrEmpty(tableAlias) ? tableName : $"{tableName} AS {tableAlias}");
        }

        public void AddJoin(string joinType, string tableName, string? tableAlias, string condition,
            bool withNoLock = false)
        {
            if (withNoLock)
                AddExpression($"{joinType} {tableName} AS {tableAlias} {NolockExpr} ON {condition}");
            else
                AddExpression($"{joinType} {tableName} AS {tableAlias} ON {condition}");
        }

        public void AddLeftJoin(string tableName, string? tableAlias, string condition, bool withNoLock = false)
        {
            AddJoin("LEFT JOIN", tableName, tableAlias, condition, withNoLock);
        }

        public void AddInnerJoin(string tableName, string? tableAlias, string condition, bool withNoLock = false)
        {
            AddJoin("INNER JOIN", tableName, tableAlias, condition, withNoLock);
        }

        private string GetSimpleCondition<TFrom, TTo, TProp>(string? tableFromAlias,
            string? tableToAlias, Expression<Func<TFrom, TProp>> fromProperty, Expression<Func<TTo, TProp>> toProperty)
        {
            return
                $"{tableToAlias}.{ColumnConverter.ToColumnName(toProperty)} = " +
                $"{tableFromAlias}.{ColumnConverter.ToColumnName(fromProperty)}";
        }

        public void AddLeftJoin<TFrom, TTo, TProp>(string tableToName, string? tableFromAlias,
            string? tableToAlias, Expression<Func<TFrom, TProp>> fromProperty, Expression<Func<TTo, TProp>> toProperty)
        {
            AddLeftJoin(tableToName, tableToAlias,
                GetSimpleCondition(tableFromAlias, tableToAlias, fromProperty, toProperty));
        }

        public void AddInnerJoin<TFrom, TTo, TProp>(string tableToName, string? tableFromAlias,
            string? tableToAlias, Expression<Func<TFrom, TProp>> fromProperty, Expression<Func<TTo, TProp>> toProperty)
        {
            AddInnerJoin(tableToName, tableToAlias,
                GetSimpleCondition(tableFromAlias, tableToAlias, fromProperty, toProperty));
        }

        public FromCollectorWrapper<T> From<T>(string tableName, string? tableAlias = null, bool withNoLock = false)
        {
            Add(tableName, tableAlias, withNoLock);
            var wrapper = FromCollectorWrapper<T>.For(this, tableAlias);
            return wrapper;
        }

        public string GetExpression()
        {
            return _stringBuilder.ToString();
        }

        public string AddFromClause(string sql)
        {
            sql += $@"
FROM
	{GetExpression()}";

            return sql;
        }
    }
}
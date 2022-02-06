using System;
using System.Linq.Expressions;
using System.Text;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class GroupByCollector
    {
        private readonly ColumnConverter _columnConverter;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public GroupByCollector(ColumnConverter columnConverter)
        {
            _columnConverter = columnConverter;
        }

        public void AddExpression(string expression)
        {
            if (_stringBuilder.Length != 0)
                _stringBuilder.Append(", ");

            _stringBuilder.Append(expression);
        }

        public void Add(string columnName, string? tableAlias = null)
        {
            if (string.IsNullOrEmpty(tableAlias) == false)
                columnName = $"{tableAlias}.{columnName}";

            AddExpression($"{columnName}");
        }

        public void Add<T, TProp>(Expression<Func<T, TProp>> property, string? alias = null)
        {
            var columnName = _columnConverter.ToColumnName(property, alias);
            Add(columnName);
        }

        public string GetExpression()
        {
            return _stringBuilder.ToString();
        }

        public string AddGroupByIfNeeded(string sql)
        {
            if (_stringBuilder.Length != 0)
                sql += $@"
GROUP BY {GetExpression()}";

            return sql;
        }

        public GroupByCollectorWrapper<T> For<T>(string? tableAlias = null)
        {
            return new GroupByCollectorWrapper<T>(this, tableAlias);
        }
    }
}
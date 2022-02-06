using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class SelectCollector
    {
        private readonly ColumnConverter _columnConverter;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private int _topNumber;

        public SelectCollector(ColumnConverter columnConverter)
        {
            _columnConverter = columnConverter;
        }

        public void SetTop(int topNumber)
        {
            _topNumber = topNumber;
        }

        public void AddExpression(string expression)
        {
            if (_stringBuilder.Length != 0)
            {
                _stringBuilder.Append(",");
                _stringBuilder.AppendLine();
                _stringBuilder.Append("	");
            }

            _stringBuilder.Append(expression);
        }

        public void Add(string columnName, string? columnAlias = null)
        {
            AddExpression(string.IsNullOrEmpty(columnAlias) ? columnName : $"{columnName} AS {columnAlias}");
        }

        public void Add<T, TProp>(string columnName, Expression<Func<T, TProp>> property)
        {
            var propertyName = property.GetPropertyName();
            Add(columnName, propertyName);
        }

        public void Add<T, TProp>(Expression<Func<T, TProp>> property, string? tableAlias = null)
        {
            var propertyName = property.GetPropertyName();
            var columnName = _columnConverter.ToColumnName(propertyName);
            if (string.IsNullOrEmpty(tableAlias) == false)
                columnName = $"{tableAlias}.{columnName}";

            Add(columnName, propertyName);
        }

        public void AddExpression(Func<ColumnConverter, string> expression)
        {
            var columnName = expression(_columnConverter);
            Add(columnName);
        }

        public void AddExpression<T, TProp>(string expression, Expression<Func<T, TProp>> property)
        {
            var propertyName = property.GetPropertyName();
            Add(expression, propertyName);
        }

        public void AddExpression<T, TProp>(Func<ColumnConverter, string> expression, Expression<Func<T, TProp>> property)
        {
            var columnName = expression(_columnConverter);
            var propertyName = property.GetPropertyName();

            Add(columnName, propertyName);
        }

        public SelectCollector AddExpression<T>(Expression<Func<T, string>> expression, string? tableAlias = null)
        {
            AddExpression(c => c.Map(expression, tableAlias));
            return this;
        }

        public void AddCount(string columnName = "cnt")
        {
            Add("COUNT(*)", columnName);
        }

        public void Add<T, TProp, TDto, TDtoProp>(Expression<Func<T, TProp>> property,
            Expression<Func<TDto, TDtoProp>> dtoProperty, string? tableAlias = null)
        {
            var propertyName = dtoProperty.GetPropertyName();
            var columnName = _columnConverter.ToColumnName(property);
            if (string.IsNullOrEmpty(tableAlias) == false)
                columnName = $"{tableAlias}.{columnName}";

            Add(columnName, propertyName);
        }

        public SelectCollectorWrapper<T> To<T>(string? tableAlias = null)
        {
            return SelectCollectorWrapper<T>.For(this, _columnConverter, tableAlias);
        }

        public string GetExpression()
        {
            return _stringBuilder.ToString();
        }

        public string GetSelectClause()
        {
            var sql = _topNumber > 0
                ? $@"
SELECT TOP {_topNumber}
	{GetExpression()}"
                : $@"
SELECT
	{GetExpression()}";

            return sql;
        }
    }
}
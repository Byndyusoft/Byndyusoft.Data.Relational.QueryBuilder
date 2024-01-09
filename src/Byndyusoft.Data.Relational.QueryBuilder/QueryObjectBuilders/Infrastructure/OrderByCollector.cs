using System;
using System.Linq.Expressions;
using System.Text;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Sort;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure.PropertyTransformers;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class OrderByCollector
    {
        private readonly ColumnConverter _columnConverter;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public OrderByCollector(ColumnConverter columnConverter)
        {
            _columnConverter = columnConverter;
        }

        public void AddExpression(string expression)
        {
            if (_stringBuilder.Length != 0)
                _stringBuilder.Append(", ");

            _stringBuilder.Append(expression);
        }

        private string GetDescendingString()
        {
            return _columnConverter.IsPostgres ? "DESC NULLS LAST" : "DESC";
        }

        private string GetAscendingString()
        {
            return _columnConverter.IsPostgres ? "ASC NULLS FIRST" : "ASC";
        }

        public void Add(string columnName, bool isDescending = false, string? tableAlias = null)
        {
            if (string.IsNullOrEmpty(tableAlias) == false)
                columnName = $"{tableAlias}.{columnName}";

            AddExpression($"{columnName} {(isDescending ? GetDescendingString() : GetAscendingString())}");
        }

        public void Add<T, TProp>(Expression<Func<T, TProp>> property, bool isDescending, string? alias = null)
        {
            var columnName = _columnConverter.ToColumnName(property, alias);
            Add(columnName, isDescending);
        }

        public void Add(PropertyTransformerItem propertyTransformerItem, SortDirection sortDirection)
        {
            Add(_columnConverter.ToColumnName(propertyTransformerItem.To), sortDirection == SortDirection.Desc,
                propertyTransformerItem.TableAlias);
        }

        public void AddBySortDescription(ISortDescription? sortDescription, PropertyTransformer propertyTransformer)
        {
            if (sortDescription != null && string.IsNullOrEmpty(sortDescription.PropertyName) == false)
            {
                var item = propertyTransformer.GetItem(sortDescription.PropertyName);
                if (item != null)
                    Add(item, sortDescription.Direction);
            }
        }

        public string GetExpression()
        {
            return _stringBuilder.ToString();
        }

        public string AddOrderByIfNeeded(string sql)
        {
            if (_stringBuilder.Length != 0)
                sql += $@"
ORDER BY {GetExpression()}";

            return sql;
        }
    }
}
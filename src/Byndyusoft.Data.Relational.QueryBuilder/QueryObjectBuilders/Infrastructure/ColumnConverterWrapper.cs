using System;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class ColumnConverterWrapper<T>
    {
        private readonly ColumnConverter _columnConverter;
        private readonly string _tableAlias;

        public ColumnConverterWrapper(ColumnConverter columnConverter, string tableAlias)
        {
            _columnConverter = columnConverter;
            _tableAlias = tableAlias;
        }

        public string To<TProp>(Expression<Func<T, TProp>> property)
        {
            var columnName = _columnConverter.ToColumnName(property);
            if (string.IsNullOrEmpty(_tableAlias) == false)
                columnName = $"{_tableAlias}.{columnName}";

            return columnName;
        }
    }
}
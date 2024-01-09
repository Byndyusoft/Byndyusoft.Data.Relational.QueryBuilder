using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class SelectCollectorWrapper<T>
    {
        private readonly ColumnConverter _columnConverter;
        private readonly SelectCollector _selectCollector;
        private readonly string? _tableAlias;

        private SelectCollectorWrapper(SelectCollector selectCollector, ColumnConverter columnConverter,
            string? tableAlias)
        {
            _selectCollector = selectCollector;
            _columnConverter = columnConverter;
            _tableAlias = tableAlias;
        }

        public static SelectCollectorWrapper<T> For(SelectCollector selectCollector, ColumnConverter columnConverter,
            string? tableAlias = null)
        {
            return new SelectCollectorWrapper<T>(selectCollector, columnConverter, tableAlias);
        }

        public SelectCollectorWrapper<T> GetExpression<TProp>(string expression, Expression<Func<T, TProp>> property)
        {
            _selectCollector.Add(expression, property);
            return this;
        }

        public SelectCollectorWrapper<T> Get<TProp>(Expression<Func<T, TProp>> property)
        {
            _selectCollector.Add(property, _tableAlias);
            return this;
        }

        public SelectCollectorWrapper<T> Get<T2, TProp>(Expression<Func<T, TProp>> property,
            Expression<Func<T2, string>> expression, string? table2Alias = null)
        {
            _selectCollector.AddExpression(_columnConverter.Map(expression, table2Alias), property);
            return this;
        }

        public SelectCollectorWrapper<T> Get<T2, T3, TProp>(Expression<Func<T, TProp>> property,
            Expression<Func<T2, T3, string>> expression, string? table2Alias = null, string? table3Alias = null)
        {
            _selectCollector.AddExpression(_columnConverter.Map(expression, table2Alias, table3Alias), property);
            return this;
        }

        public SelectCollectorWrapper<T> Get<T2, T3, T4, TProp>(Expression<Func<T, TProp>> property,
            Expression<Func<T2, T3, T4, string>> expression, string? table2Alias = null, string? table3Alias = null,
            string? table4Alias = null)
        {
            _selectCollector.AddExpression(_columnConverter.Map(expression, table2Alias, table3Alias, table4Alias),
                property);
            return this;
        }

        public SelectCollectorWrapper<T> Get<T2, T3, T4, T5, TProp>(Expression<Func<T, TProp>> property,
            Expression<Func<T2, T3, T4, T5, string>> expression, string? table2Alias = null, string? table3Alias = null,
            string? table4Alias = null, string? table5Alias = null)
        {
            _selectCollector.AddExpression(
                _columnConverter.Map(expression, table2Alias, table3Alias, table4Alias, table5Alias), property);
            return this;
        }

        public SelectCollectorWrapper<T> GetValues(IEnumerable<string> propertyNames)
        {
            foreach (var publicPropertyName in propertyNames)
            {
                var columnName = _columnConverter.ToColumnName(publicPropertyName);
                if (string.IsNullOrEmpty(_tableAlias) == false)
                    columnName = $"{_tableAlias}.{columnName}";
                _selectCollector.Add(columnName, publicPropertyName);
            }

            return this;
        }

        public SelectCollectorWrapper<T> GetAllPublicValues()
        {
            return GetValues(TypeCache<T>.GetPublicPropertyNames());
        }

        public SelectCollector Else()
        {
            return _selectCollector;
        }

        public SelectCollectorWrapper<TFrom, T> From<TFrom>(string? tableAlias = null)
        {
            return SelectCollectorWrapper<TFrom, T>.For(_selectCollector, _columnConverter, this, tableAlias);
        }

        public string GetSelectClause()
        {
            return _selectCollector.GetSelectClause();
        }
    }

    public class SelectCollectorWrapper<T, TDto>
    {
        private readonly SelectCollectorWrapper<TDto>? _baseSelectCollectorWrapper;
        private readonly ColumnConverter _columnConverter;
        private readonly SelectCollector _selectCollector;
        private readonly string? _tableAlias;

        private SelectCollectorWrapper(SelectCollector selectCollector, ColumnConverter columnConverter,
            SelectCollectorWrapper<TDto>? baseSelectCollectorWrapper, string? tableAlias)
        {
            _selectCollector = selectCollector;
            _columnConverter = columnConverter;
            _baseSelectCollectorWrapper = baseSelectCollectorWrapper;
            _tableAlias = tableAlias;
        }

        public static SelectCollectorWrapper<T, TDto> For(SelectCollector selectCollector,
            ColumnConverter columnConverter, string? tableAlias = null)
        {
            return new SelectCollectorWrapper<T, TDto>(selectCollector, columnConverter, null, tableAlias);
        }

        public static SelectCollectorWrapper<T, TDto> For(SelectCollector selectCollector,
            ColumnConverter columnConverter,
            SelectCollectorWrapper<TDto>? baseSelectCollectorWrapper, string? tableAlias = null)
        {
            return new SelectCollectorWrapper<T, TDto>(selectCollector, columnConverter, baseSelectCollectorWrapper,
                tableAlias);
        }

        public SelectCollectorWrapper<T, TDto> Get<TProp, TDtoProp>(Expression<Func<T, TProp>> property,
            Expression<Func<TDto, TDtoProp>> dtoProperty)
        {
            _selectCollector.Add(property, dtoProperty, _tableAlias);
            return this;
        }

        public SelectCollectorWrapper<T, TDto> GetExpression<TProp>(string expression,
            Expression<Func<TDto, TProp>> property)
        {
            _selectCollector.AddExpression(expression, property);
            return this;
        }

        public SelectCollectorWrapper<T, TDto> GetExpression<TProp>(Expression<Func<T, string>> expression,
            Expression<Func<TDto, TProp>> property)
        {
            _selectCollector.AddExpression(c => c.Map(expression, _tableAlias), property);
            return this;
        }

        public SelectCollector Else()
        {
            return _selectCollector;
        }

        public SelectCollectorWrapper<T, TDto> GetValues(IEnumerable<string> propertyNames)
        {
            foreach (var publicPropertyName in propertyNames)
            {
                var columnName = _columnConverter.ToColumnName(publicPropertyName, _tableAlias);
                _selectCollector.Add(columnName, publicPropertyName);
            }

            return this;
        }

        public SelectCollectorWrapper<T, TDto> GetAllPublicValues(
            Func<IEnumerable<string>, IEnumerable<string>>? transformer = null)
        {
            var publicPropertyNames = TypeCache<T>.GetPublicPropertyNames();
            if (transformer != null)
                publicPropertyNames = transformer(publicPropertyNames);

            return GetValues(publicPropertyNames);
        }

        public SelectCollectorWrapper<TOther, TDto> Other<TOther>(string tableAlias)
        {
            return SelectCollectorWrapper<TOther, TDto>.For(_selectCollector, _columnConverter,
                _baseSelectCollectorWrapper, tableAlias);
        }

        public string GetSelectClause()
        {
            return _selectCollector.GetSelectClause();
        }
    }
}
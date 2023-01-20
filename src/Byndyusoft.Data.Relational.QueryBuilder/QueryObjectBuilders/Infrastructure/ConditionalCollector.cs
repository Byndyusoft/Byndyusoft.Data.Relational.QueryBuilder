using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class ConditionalCollector
    {
        private readonly string _paramPrefix;
        private int _counter;
        private int _prefixCounter;

        public ConditionalCollector(ColumnConverter columnConverter, string? paramPrefix = null)
        {
            _paramPrefix = paramPrefix ?? "_cc";
            ColumnConverter = columnConverter;
        }

        public ColumnConverter ColumnConverter { get; }
        private DynamicParameters Parameters { get; } = new DynamicParameters();
        public List<ConditionalCollectorChunk> Chunks { get; } = new List<ConditionalCollectorChunk>();

        public IEnumerable<DynamicParameters> GetArrayParameters()
        {
            yield return Parameters;
            foreach (var conditionalCollectorChunk in Chunks)
                foreach (var parameters in conditionalCollectorChunk.GetParameters())
                    yield return parameters;
        }

        public ConditionalCollector GetNextOrConditionalCollector()
        {
            var prefix = $"{_paramPrefix}_{_prefixCounter++}";
            return new ConditionalCollector(ColumnConverter, prefix);
        }

        public DynamicParameters GetParameters()
        {
            var dynamicParameters = new DynamicParameters();
            foreach (var parameters in GetArrayParameters()) dynamicParameters.AddDynamicParams(parameters);

            return dynamicParameters;
        }

        public void Add(string conditional, object? template)
        {
            Chunks.Add(new ConditionalCollectorStringChunk(conditional));
            if (template != null)
                Parameters.AddDynamicParams(template);
        }

        public void AddOr(params ConditionalCollector[] conditionalCollectors)
        {
            Chunks.Add(new ConditionalCollectorOrChunk(conditionalCollectors));
        }

        public void AddOr(params Action<ConditionalCollector>[] orConditions)
        {
            if (orConditions == null)
                throw new ArgumentNullException(nameof(orConditions));

            var conditionalCollectorWrappers = orConditions.Select(AddOrSingle).ToArray();
            AddOr(conditionalCollectorWrappers);
        }

        private ConditionalCollector AddOrSingle(Action<ConditionalCollector> orCondition)
        {
            var wrapper = GetNextOrConditionalCollector();
            orCondition(wrapper);
            return wrapper;
        }

        public void AddParamValue(string paramName, object value)
        {
            Parameters.AddParamValue(paramName, value);
        }

        public void AddDynamicParams(object subqueryQueryParams)
        {
            Parameters.AddDynamicParams(subqueryQueryParams);
        }

        public void AddFormattableString(FormattableString conditional, params int[] constIndices)
        {
            var template = new DynamicParameters();
            var arguments = new List<object?>();

            for (var i = 0; i < conditional.ArgumentCount; ++i)
            {
                var arg = conditional.GetArgument(i);
                if (constIndices.Contains(i))
                {
                    arguments.Add(arg);
                }
                else
                {
                    var argumentName = _paramPrefix + ++_counter;
                    arguments.Add("@" + argumentName);
                    template.AddParamValue(argumentName, ColumnConverter.ToArgument(arg));
                }
            }

            var sql = string.Format(conditional.Format, arguments.ToArray());
            Add(sql, template);
        }

        public void AddExpression<T>(Expression<Func<T, string>> formatString, object? template = null, string? tableAlias = null)
        {
            Add(ColumnConverter.Map(formatString, tableAlias), template);
        }

        public void AddExpression<T, T2>(Expression<Func<T, T2, string>> formatString, object? template = null,
            string? tableAlias = null, string? table2Alias = null)
        {
            Add(ColumnConverter.Map(formatString, tableAlias, table2Alias), template);
        }

        public void AddExpression<T, T2, T3>(Expression<Func<T, T2, T3, string>> formatString, object? template = null,
            string? tableAlias = null, string? table2Alias = null, string? table3Alias = null)
        {
            Add(ColumnConverter.Map(formatString, tableAlias, table2Alias, table3Alias), template);
        }

        public void AddNull<T, TProp>(Expression<Func<T, TProp>> func, string? tableAlias = null)
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            Add($"{columnName} IS NULL", null);
        }

        public void AddNull<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            columnName = ColumnConverter.ToColumnName(columnName, tableAlias);
            Add($"{columnName} IS NULL", null);
        }

        public void AddNotNull<T, TProp>(Expression<Func<T, TProp>> func, string? tableAlias = null)
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            Add($"{columnName} IS NOT NULL", null);
        }

        public void AddNotNull<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            columnName = ColumnConverter.ToColumnName(columnName, tableAlias);
            Add($"{columnName} IS NOT NULL", null);
        }

        private void AddBinaryImpl<TProp>(string columnName, TProp value, string binaryOperator)
        {
            AddFormattableString($"{columnName} {binaryOperator} {value}", 0, 1);
        }

        public void AddBinary<TProp>(string columnName, TProp value, string binaryOperator, string? tableAlias = null)
        {
            columnName = ColumnConverter.ToColumnName(columnName, tableAlias);
            AddBinaryImpl(columnName, value, binaryOperator);
        }

        public void AddBinary<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string binaryOperator,
            string? tableAlias = null)
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            AddBinaryImpl(columnName, value, binaryOperator);
        }

        public void AddEquals<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            AddBinary(columnName, value, "=", tableAlias);
        }

        public void AddEquals<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, "=", tableAlias);
        }

        public void AddEquals<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, "=", tableAlias);
        }

        public void AddNotEquals<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, "!=", tableAlias);
        }

        public void AddNotEquals<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, "!=", tableAlias);
        }

        public void AddLess<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, "<", tableAlias);
        }

        public void AddLess<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, "<", tableAlias);
        }

        public void AddLess<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            AddBinary(columnName, value, "<", tableAlias);
        }

        public void AddLessOrEqual<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, "<=", tableAlias);
        }

        public void AddLessOrEqual<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, "<=", tableAlias);
        }

        public void AddLessOrEqual<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            AddBinary(columnName, value, "<=", tableAlias);
        }

        public void AddGreater<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, ">", tableAlias);
        }

        public void AddGreater<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, ">", tableAlias);
        }

        public void AddGreater<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            AddBinary(columnName, value, ">", tableAlias);
        }

        public void AddGreaterOrEqual<T, TProp>(Expression<Func<T, TProp>> func, TProp value, string? tableAlias = null)
        {
            AddBinary(func, value, ">=", tableAlias);
        }

        public void AddGreaterOrEqual<T, TProp>(Expression<Func<T, TProp?>> func, TProp value, string? tableAlias = null)
            where TProp : struct
        {
            AddBinary(func, value, ">=", tableAlias);
        }

        public void AddGreaterOrEqual<TProp>(string columnName, TProp value, string? tableAlias = null)
        {
            AddBinary(columnName, value, ">=", tableAlias);
        }

        public void AddInForExpression<TProp>(string expression, IReadOnlyCollection<TProp> values)
        {
            if (ColumnConverter.IsPostgres == false)
                AddFormattableString($"{expression} IN {values.ToArray()}", 0);
            else
                AddFormattableString($"{expression} = ANY(ARRAY[{values.ToArray()}])", 0);
        }

        public void AddIn<TProp>(string columnName, IReadOnlyCollection<TProp> values, string? tableAlias = null)
        {
            var columnNameWithAlias = ColumnConverter.ToColumnName(columnName, tableAlias);
            AddInForExpression(columnNameWithAlias, values);
        }

        public void AddIn<T, TProp>(Expression<Func<T, TProp>> func, IReadOnlyCollection<TProp> values, string? tableAlias = null)
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            AddInForExpression(columnName, values);
        }

        public void AddIn<T, TProp>(Expression<Func<T, TProp?>> func, IReadOnlyCollection<TProp> values, string? tableAlias = null)
            where TProp : struct
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            AddInForExpression(columnName, values);
        }

        public void AddNotInForExpression<TProp>(string expression, IReadOnlyCollection<TProp> values)
        {
            if (ColumnConverter.IsPostgres == false)
                AddFormattableString($"{expression} NOT IN {values.ToArray()}", 0);
            else
                AddFormattableString($"{expression} != ALL(ARRAY[{values.ToArray()}])", 0);
        }

        public void AddNotIn<TProp>(string columnName, IReadOnlyCollection<TProp> values, string? tableAlias = null)
        {
            var columnNameWithAlias = ColumnConverter.ToColumnName(columnName, tableAlias);
            AddNotInForExpression(columnNameWithAlias, values);
        }

        public void AddNotIn<T, TProp>(Expression<Func<T, TProp>> func, IReadOnlyCollection<TProp> values, string? tableAlias = null)
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            AddNotInForExpression(columnName, values);
        }

        public void AddNotIn<T, TProp>(Expression<Func<T, TProp?>> func, IReadOnlyCollection<TProp> values, string? tableAlias = null)
            where TProp : struct
        {
            var columnName = ColumnConverter.ToColumnName(func, tableAlias);
            AddNotInForExpression(columnName, values);
        }

        public ConditionalCollectorWrapper<T> For<T>(string? tableAlias = null)
        {
            return new ConditionalCollectorWrapper<T>(this, tableAlias);
        }

        public string GetConditional()
        {
            return string.Join(" AND ", Chunks.Select(x => "(" + x.GetConditional() + ")"));
        }

        public bool Empty()
        {
            return Chunks.Count == 0;
        }

        public string AddWhereIfNeeded(string sql)
        {
            if (Empty() == false)
                sql += $@"
WHERE {GetConditional()}";

            return sql;
        }
    }

    public abstract class ConditionalCollectorChunk
    {
        public abstract string GetConditional();

        public abstract IEnumerable<DynamicParameters> GetParameters();
    }

    public class ConditionalCollectorStringChunk : ConditionalCollectorChunk
    {
        private readonly string _condition;

        public ConditionalCollectorStringChunk(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                throw new ArgumentNullException(nameof(condition));

            _condition = condition;
        }

        public override string GetConditional()
        {
            return _condition;
        }

        public override IEnumerable<DynamicParameters> GetParameters()
        {
            yield break;
        }
    }

    public class ConditionalCollectorOrChunk : ConditionalCollectorChunk
    {
        private readonly ConditionalCollector[] _collectors;


        public ConditionalCollectorOrChunk(params ConditionalCollector[] collectors)
        {
            if (collectors == null || collectors.Length < 1)
                throw new ArgumentException("There must be at least one OR condition", nameof(collectors));

            _collectors = collectors;
        }

        public override string GetConditional()
        {
            return string.Join(" OR ", _collectors.Select(i => $"({i.GetConditional()})"));
        }

        public override IEnumerable<DynamicParameters> GetParameters()
        {
            return _collectors.SelectMany(i => i.GetArrayParameters());
        }
    }
}
using Dapper;
using System.Collections.Generic;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public class ValueCollector
    {
        private readonly ColumnConverter _columnConverter;
        private readonly string _paramNamePrefix;

        private int _counter;

        public ValueCollector(ColumnConverter columnConverter, string paramNamePrefix = "@_vc")
        {
            _columnConverter = columnConverter;
            _paramNamePrefix = paramNamePrefix;
        }

        public DynamicParameters ParametersValues { get; } = new DynamicParameters();
        public List<FieldParameter> FieldParameters { get; } = new List<FieldParameter>();

        public void Add(string field, object? value)
        {
            var paramName = _paramNamePrefix + ++_counter;

            ParametersValues.AddParamValue(paramName, _columnConverter.ToArgument(value));
            FieldParameters.Add(new FieldParameter(field, paramName));
        }

        // Метод нужен, когда подается не параметр, а, например, вычисляется
        public void AddExpression(string field, string value)
        {
            FieldParameters.Add(new FieldParameter(field, value));
        }
    }

    public class FieldParameter
    {
        public FieldParameter(string field, string paramName)
        {
            Field = field;
            ParamName = paramName;
        }

        public string Field { get; }
        public string ParamName { get; }
    }
}
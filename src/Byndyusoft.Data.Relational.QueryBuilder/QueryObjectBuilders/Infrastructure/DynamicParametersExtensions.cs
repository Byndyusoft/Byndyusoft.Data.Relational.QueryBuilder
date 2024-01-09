using System;
using System.Data;
using Dapper;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public static class DynamicParametersExtensions
    {
        //Если добавляется одиночное значение, то нужно использовать этот метод, 
        //т.к. он корретно работает с датами и перечисленями
        public static void AddParamValue(this DynamicParameters dynamicParameters, string paramName, object? value)
        {
            if (value is DateTimeOffset)
                dynamicParameters.Add(paramName, value, DbType.DateTimeOffset);
            else if (value is Enum && Enum.IsDefined(value.GetType(), value))
                dynamicParameters.Add(paramName, value.ToString());
            else
                dynamicParameters.Add(paramName, value);
        }
    }
}
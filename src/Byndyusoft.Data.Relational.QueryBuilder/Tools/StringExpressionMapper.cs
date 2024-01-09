using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Byndyusoft.Data.Relational.QueryBuilder.Tools
{
    public static class StringExpressionMapper
    {
        private static readonly MethodInfo[] Methods;

        static StringExpressionMapper()
        {
            var type = typeof(string);
            Methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(i =>
                i.Name == nameof(string.Format) &&
                i.GetParameters()[0].ParameterType ==
                typeof(string)).ToArray();
        }

        public static string Map<T>(Expression<Func<T, string>> formatter,
            Func<string, string>? memberTransformer = null)
        {
            return Map(formatter, new[] { memberTransformer });
        }

        public static string Map<T1, T2>(Expression<Func<T1, T2, string>> formatter,
            Func<string, string>? memberTransformer1 = null, Func<string, string>? memberTransformer2 = null)
        {
            return Map(formatter, new[] { memberTransformer1, memberTransformer2 });
        }

        public static string Map<T1, T2, T3>(Expression<Func<T1, T2, T3, string>> formatter,
            Func<string, string>? memberTransformer1 = null, Func<string, string>? memberTransformer2 = null,
            Func<string, string>? memberTransformer3 = null)
        {
            return Map(formatter, new[] { memberTransformer1, memberTransformer2, memberTransformer3 });
        }

        public static string Map<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, string>> formatter,
            Func<string, string>? memberTransformer1 = null, Func<string, string>? memberTransformer2 = null,
            Func<string, string>? memberTransformer3 = null, Func<string, string>? memberTransformer4 = null)
        {
            return Map(formatter,
                new[] { memberTransformer1, memberTransformer2, memberTransformer3, memberTransformer4 });
        }

        private static string Map(LambdaExpression expression, Func<string, string>?[] memberTransformers)
        {
            if (expression.Body is ConstantExpression constantBodyExpr)
                return constantBodyExpr.Value?.ToString() ?? string.Empty;

            var parameters = expression.Parameters.ToArray();
            if (parameters.Length != memberTransformers.Length)
                throw new Exception("Невозможная ошибка");

            var formatMethodExpr = expression.Body as MethodCallExpression;
            if (formatMethodExpr == null || Methods.Contains(formatMethodExpr.Method) == false)
                throw new Exception($"Body {expression.Body} is not one of string.format methods");

            var formatExpr = formatMethodExpr.Arguments[0] as ConstantExpression;
            if (formatExpr == null)
                throw new Exception($"Format {formatMethodExpr.Arguments[0]} is not constant");

            var formatString = (string)formatExpr.Value;

            var formatMethodArgs = new List<Expression>();
            if (formatMethodExpr.Arguments.Count == 2 && formatMethodExpr.Arguments[1].Type == typeof(object[]))
            {
                var argumentArrayExpr = formatMethodExpr.Arguments[1] as NewArrayExpression;
                if (argumentArrayExpr == null)
                    throw new Exception($"Format {formatMethodExpr.Arguments[1]} is not array expression");

                formatMethodArgs.AddRange(argumentArrayExpr.Expressions);
            }
            else
            {
                formatMethodArgs.AddRange(formatMethodExpr.Arguments.Skip(1));
            }

            var arguments = new List<string?>();

            foreach (var formatMethodArg in formatMethodArgs)
            {
                if (formatMethodArg.Type.IsValueType)
                    throw new Exception(
                        $"Type of argument expression {formatMethodArg} is value type with not convert");

                var argumentName = GetArgumentFromExpression(formatMethodArg, parameters, memberTransformers);
                arguments.Add(argumentName);
            }

            return string.Format(formatString, arguments.Cast<object>().ToArray());
        }

        private static string? GetArgumentFromExpression(Expression expression, ParameterExpression[] parameters,
            Func<string, string>?[] memberTransformers)
        {
            if (expression is ConstantExpression constantArgExpr)
                return GetArgument(constantArgExpr);

            if (expression is MemberExpression memberArgExpr)
                return GetArgument(memberArgExpr, parameters, memberTransformers);

            if (expression is UnaryExpression unaryArgExpr)
                return GetArgument(unaryArgExpr, parameters, memberTransformers);

            throw new Exception(
                $"Argument expression type {expression.GetType().Name} is not allowed for {expression}");
        }

        private static string? GetArgument(ConstantExpression expression)
        {
            return expression.Value?.ToString();
        }

        private static string? GetArgument(MemberExpression expression, ParameterExpression[] parameters,
            Func<string, string>?[] memberTransformers)
        {
            if (expression.Expression == null || expression.Expression is ConstantExpression)
            {
                var constantExpression = expression.Expression as ConstantExpression;
                object? value;
                if (expression.Member is FieldInfo fieldInfo)
                    value = fieldInfo.GetValue(constantExpression?.Value);
                else if (expression.Member is PropertyInfo propertyInfo)
                    value = propertyInfo.GetValue(constantExpression?.Value);
                else
                    throw new Exception(
                        $"Member info of type {expression.Member.GetType().Name} is not supported to get constant value");

                return value?.ToString();
            }

            Func<string, string>? transformer = null;
            var found = false;
            for (var i = 0; i < parameters.Length; i++)
                if (parameters[i] == GetParameterExpression(expression.Expression))
                {
                    transformer = memberTransformers[i];
                    found = true;
                    break;
                }

            if (found == false)
                throw new Exception($"Only one level of member getting from parameter is allowed for {expression}");

            var name = expression.Member.Name;
            if (transformer != null)
                name = transformer(name);
            return name;
        }

        private static Expression GetParameterExpression(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
                return GetConvertOperandExpression(unaryExpression);

            return expression;
        }

        private static string? GetArgument(UnaryExpression expression, ParameterExpression[] parameters,
            Func<string, string>?[] memberTransformers)
        {
            var operandExpression = GetConvertOperandExpression(expression);
            return GetArgumentFromExpression(operandExpression, parameters, memberTransformers);
        }

        private static Expression GetConvertOperandExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType != ExpressionType.Convert)
                throw new Exception($"Only conversion is allowed for {unaryExpression}");

            return unaryExpression.Operand;
        }
    }
}
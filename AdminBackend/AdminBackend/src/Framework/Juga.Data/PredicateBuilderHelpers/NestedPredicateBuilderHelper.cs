using LinqKit;
using System.Linq.Expressions;
using Juga.Data.Enums;

namespace Juga.Data.PredicateBuilderHelpers;

public static class NestedPredicateBuilderHelper
{
    public static ExpressionStarter<T> BuildPredicate<T>(object requestModel, Dictionary<string, (string Path, Operators Operator)> operators = null)
    {
        var predicate = PredicateBuilder.New<T>(true);
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var property in requestModel.GetType().GetProperties())
        {
            var value = property.GetValue(requestModel);

            if (value == null || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)))
                continue;

            Expression propertyExpression;
            var constantExpression = Expression.Constant(value);

            if (operators != null && operators.TryGetValue(property.Name, out var opInfo))
            {
                propertyExpression = GetNestedPropertyExpression(parameter, opInfo.Path);
                constantExpression = Expression.Constant(value, propertyExpression.Type);
                ApplyOperator(ref predicate, opInfo.Operator, propertyExpression, constantExpression, parameter);
            }
            else
            {
                // Default to equality with property name matching
                propertyExpression = GetNestedPropertyExpression(parameter, property.Name);
                constantExpression = Expression.Constant(value, propertyExpression.Type);
                ApplyOperator(ref predicate, Operators.Equal, propertyExpression, constantExpression, parameter);
            }
        }

        return predicate;
    }

    private static void ApplyOperator<T>(ref ExpressionStarter<T> predicate, Operators op, Expression propertyExpression, Expression constantExpression, ParameterExpression parameter)
    {
        Expression comparisonExpression = null;

        switch (op)
        {
            case Operators.Equal:
                comparisonExpression = Expression.Equal(propertyExpression, constantExpression);
                break;
            case Operators.GreaterThan:
                comparisonExpression = Expression.GreaterThan(EnsureSameType(propertyExpression, constantExpression), constantExpression);
                break;
            case Operators.GreaterThanOrEqual:
                comparisonExpression = Expression.GreaterThanOrEqual(EnsureSameType(propertyExpression, constantExpression), constantExpression);
                break;
            case Operators.LessThan:
                comparisonExpression = Expression.LessThan(EnsureSameType(propertyExpression, constantExpression), constantExpression);
                break;
            case Operators.LessThanOrEqual:
                comparisonExpression = Expression.LessThanOrEqual(EnsureSameType(propertyExpression, constantExpression), constantExpression);
                break;
            case Operators.Contains:
                if (propertyExpression.Type == typeof(string))
                {
                    comparisonExpression = Expression.Call(propertyExpression, nameof(string.Contains), null, constantExpression);
                }
                break;
            case Operators.StartsWith:
                if (propertyExpression.Type == typeof(string))
                {
                    comparisonExpression = Expression.Call(propertyExpression, nameof(string.StartsWith), null, constantExpression);
                }
                break;
            case Operators.EndsWith:
                if (propertyExpression.Type == typeof(string))
                {
                    comparisonExpression = Expression.Call(propertyExpression, nameof(string.EndsWith), null, constantExpression);
                }
                break;
            case Operators.Between:
                if (constantExpression is ConstantExpression constant && constant.Value is Tuple<object, object> tuple)
                {
                    var minExpression = Expression.Constant(tuple.Item1, propertyExpression.Type);
                    var maxExpression = Expression.Constant(tuple.Item2, propertyExpression.Type);
                    comparisonExpression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(EnsureSameType(propertyExpression, minExpression), minExpression),
                        Expression.LessThanOrEqual(EnsureSameType(propertyExpression, maxExpression), maxExpression)
                    );
                }
                break;
            default:
                throw new InvalidOperationException($"Unsupported operator: {op}");
        }

        if (comparisonExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
            predicate = predicate.And(lambda);
        }
    }

    private static Expression GetNestedPropertyExpression(Expression parameter, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        Expression expression = parameter;

        foreach (var property in properties)
        {
            expression = Expression.Property(expression, property);
        }

        return expression;
    }

    private static Expression EnsureSameType(Expression left, Expression right)
    {
        if (left.Type != right.Type)
        {
            if (left.Type.IsGenericType && left.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                right = Expression.Convert(right, left.Type);
            }
            else if (right.Type.IsGenericType && right.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                left = Expression.Convert(left, right.Type);
            }
            else
            {
                throw new InvalidOperationException("Incompatible types for comparison");
            }
        }
        return left;
    }
}
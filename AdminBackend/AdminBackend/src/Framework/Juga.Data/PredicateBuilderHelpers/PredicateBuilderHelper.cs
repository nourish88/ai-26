using System.Linq.Expressions;
using LinqKit;
using Juga.Data.Enums;

namespace Juga.Data.PredicateBuilderHelpers;

public static class PredicateBuilderHelper
{

    public static ExpressionStarter<T> BuildPredicate<T>(object requestModel, Dictionary<string, (string EntityProperty, Operators Operator)> propertyMappings = null)
    {
        var predicate = PredicateBuilder.New<T>(true);
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var property in requestModel.GetType().GetProperties())
        {
            var value = property.GetValue(requestModel);

            if (value == null || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)))
                continue;

            string entityPropertyName = property.Name;
            Operators operatorType = Operators.Equal; // Default to equality

            if (propertyMappings != null && propertyMappings.TryGetValue(property.Name, out var mapping))
            {
                entityPropertyName = mapping.EntityProperty;
                operatorType = mapping.Operator;
            }

            var propertyExpression = Expression.Property(parameter, entityPropertyName);
            var constantExpression = Expression.Constant(value, propertyExpression.Type);
            Expression comparisonExpression = null;

            switch (operatorType)
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
                    if (value is Tuple<object, object> tuple)
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
                    throw new InvalidOperationException($"Unsupported operator: {operatorType}");
            }

            if (comparisonExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
                predicate = predicate.And(lambda);
            }
        }

        return predicate;
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
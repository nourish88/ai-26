
using Juga.Data.Enums;
using LinqKit;

namespace Juga.Data.Extensions;

public static class PredicateBuilderExtensions
{
    /// <summary>
    /// Belirtilen istek modeline ve operatörler sözlüğüne göre iç içe geçmiş bir koşul oluşturur.
    /// Bu yöntem iç içe geçmiş özellikler üzerinden sorgulama yapılmasını sağlar.
    /// </summary>
    /// <typeparam name="T">Varlık türü.</typeparam>
    /// <param name="source">IQueryable kaynağı.</param>
    /// <param name="filterModel">Filtre kriterlerini içeren istek modeli.</param>
    /// <param name="operators">Her özellik için yol ve operatörü belirten bir sözlük.</param>
    /// <returns>Oluşturulan koşulu içeren bir ExpressionStarter.</returns>
    /// <remarks>
    /// Bu yöntem, özelliklerin iç içe geçtiği durumlarda gelişmiş filtreleme yapmak için kullanılır.
    /// </remarks>
    public static ExpressionStarter<T> BuildNestedPredicate<T>(this IQueryable<T> source, object filterModel, Dictionary<string, (string Path, Operators Operator)> operators = null)
    {
        var predicate = PredicateBuilder.New<T>(true);
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var property in filterModel.GetType().GetProperties())
        {
            var value = property.GetValue(filterModel);

            if (value == null || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)))
                continue;

            Expression propertyExpression;
            var constantExpression = Expression.Constant(value);

            if (operators != null && operators.TryGetValue(property.Name, out var opInfo))
            {
                propertyExpression = GetNestedPropertyExpression(parameter, opInfo.Path);
                if (opInfo.Operator == Operators.Between)
                {
                    ApplyBetweenOperator(ref predicate, opInfo.Operator, propertyExpression, parameter, value);
                }
                else
                {
                    constantExpression = Expression.Constant(value, propertyExpression.Type);
                    ApplyOperator(ref predicate, opInfo.Operator, propertyExpression, constantExpression, parameter);
                }
            }
            else
            {
                propertyExpression = GetNestedPropertyExpression(parameter, property.Name);
                constantExpression = Expression.Constant(value, propertyExpression.Type);
                ApplyOperator(ref predicate, Operators.Equal, propertyExpression, constantExpression, parameter);
            }
        }

        return predicate;
    }

    /// <summary>
    /// Belirtilen istek modeline ve operatörler sözlüğüne göre basit bir koşul oluşturur.
    /// Bu yöntem, düz özellikler için uygundur.
    /// </summary>
    /// <typeparam name="T">Varlık türü.</typeparam>
    /// <param name="source">IQueryable kaynağı.</param>
    /// <param name="requestModel">Filtre kriterlerini içeren istek modeli.</param>
    /// <param name="operators">Her özellik için yol ve operatörü belirten bir sözlük.</param>
    /// <returns>Oluşturulan koşulu içeren bir ExpressionStarter.</returns>
    /// <remarks>
    /// Özelliklerin iç içe geçmediği durumlarda bu yöntemi kullanın.
    /// </remarks>
    public static ExpressionStarter<T> BuildPredicate<T>(this IQueryable<T> source, object requestModel, Dictionary<string, (string Path, Operators Operator)> operators = null)
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
                if (opInfo.Operator == Operators.Between)
                {
                    ApplyBetweenOperator(ref predicate, opInfo.Operator, propertyExpression, parameter, value);
                }
                else
                {
                    constantExpression = Expression.Constant(value, propertyExpression.Type);
                    ApplyOperator(ref predicate, opInfo.Operator, propertyExpression, constantExpression, parameter);
                }
            }
            else
            {
                propertyExpression = GetNestedPropertyExpression(parameter, property.Name);
                constantExpression = Expression.Constant(value, propertyExpression.Type);
                ApplyOperator(ref predicate, Operators.Equal, propertyExpression, constantExpression, parameter);
            }
        }

        return predicate;
    }

    /// <summary>
    /// Belirtilen operatörü özellik ve sabit ifadelerine uygular, koşulu günceller.
    /// </summary>
    /// <typeparam name="T">Varlık türü.</typeparam>
    /// <param name="predicate">Mevcut koşul.</param>
    /// <param name="op">Uygulanacak operatör.</param>
    /// <param name="propertyExpression">Özellik ifadesi.</param>
    /// <param name="constantExpression">Sabit ifade.</param>
    /// <param name="parameter">Parametre ifadesi.</param>
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
            default:
                throw new InvalidOperationException($"Desteklenmeyen operatör: {op}");
        }

        if (comparisonExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
            predicate = predicate.And(lambda);
        }
    }

    /// <summary>
    /// 'Between' operatörünü özellik ifadesine uygular, koşulu günceller.
    /// </summary>
    /// <typeparam name="T">Varlık türü.</typeparam>
    /// <param name="predicate">Mevcut koşul.</param>
    /// <param name="op">Operatör (yalnızca 'Between' olmalı).</param>
    /// <param name="propertyExpression">Özellik ifadesi.</param>
    /// <param name="parameter">Parametre ifadesi.</param>
    /// <param name="value">Aralık değerlerini içeren tuple.</param>
    /// <remarks>
    /// Değer, aralık değerlerini içeren bir Tuple olmalıdır.
    /// </remarks>
    private static void ApplyBetweenOperator<T>(ref ExpressionStarter<T> predicate, Operators op, Expression propertyExpression, ParameterExpression parameter, object value)
    {
        var valueType = value.GetType();
        if (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != typeof(Tuple<,>))
        {
            throw new ArgumentException("'Between' operatörü için değer, iki değeri içeren bir Tuple olmalıdır.");
        }

        var item1 = valueType.GetProperty("Item1").GetValue(value);
        var item2 = valueType.GetProperty("Item2").GetValue(value);

        var minExpression = Expression.Constant(item1, propertyExpression.Type);
        var maxExpression = Expression.Constant(item2, propertyExpression.Type);

        var comparisonExpression = Expression.AndAlso(
            Expression.GreaterThanOrEqual(EnsureSameType(propertyExpression, minExpression), minExpression),
            Expression.LessThanOrEqual(EnsureSameType(propertyExpression, maxExpression), maxExpression)
        );

        var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        predicate = predicate.And(lambda);
    }

    /// <summary>
    /// Belirtilen özellik yoluna göre iç içe geçmiş özellik ifadesini döndürür.
    /// </summary>
    /// <param name="parameter">Parametre ifadesi.</param>
    /// <param name="propertyPath">Özellik yolu (nokta ile ayrılmış).</param>
    /// <returns>İç içe geçmiş özellik ifadesi.</returns>
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

    /// <summary>
    /// Sol ve sağ ifadelerin aynı türde olmasını sağlar.
    /// </summary>
    /// <param name="left">Sol ifade.</param>
    /// <param name="right">Sağ ifade.</param>
    /// <returns>Sol ifade, sağ ifadenin türüne dönüştürülmüş olabilir.</returns>
    /// <remarks>
    /// Bu yöntem, uyumluluğu sağlamak için nullable türleri ele alır.
    /// </remarks>
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
                throw new InvalidOperationException("Karşılaştırma için uyumsuz türler");
            }
        }
        return left;
    }
}
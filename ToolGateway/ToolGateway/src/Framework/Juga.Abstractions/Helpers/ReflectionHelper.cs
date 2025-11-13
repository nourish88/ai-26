namespace Juga.Abstractions.Helpers;

public static class ReflectionHelper
{
    public static object GetPropertyValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}
using Juga.Abstractions.Helpers;

namespace Juga.Abstractions.Caching.CacheManagement;

public class ModelPropertySuffixSelector : ICacheKeySuffixSelector
{
    public ModelPropertySuffixSelector(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

        PropertyName = propertyName;
    }

    public string PropertyName { get; set; }

    public string GetSuffix(object[] arguments)
    {
        if (arguments == null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

        var propertyValue = ReflectionHelper.GetPropertyValue(arguments[0], PropertyName);
        if (propertyValue == null) throw new ArgumentNullException($"Value of {PropertyName} is null");

        return propertyValue.ToString();
    }
}
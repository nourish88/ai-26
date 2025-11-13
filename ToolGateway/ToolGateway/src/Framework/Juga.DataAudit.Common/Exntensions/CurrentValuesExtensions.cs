

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Juga.DataAudit.Common.Exntensions;

public static class CurrentValuesExtensions
{
    public static T GetValue<T>(this PropertyValues currentValues, IProperty property)
    {
        return (T)currentValues[property];
    }
}
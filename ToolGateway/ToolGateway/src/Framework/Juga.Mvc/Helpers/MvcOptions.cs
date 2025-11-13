
using System.Reflection;

namespace Juga.Mvc.Helpers;

public class MvcOptions
{
    /// <summary>
    ///     FluentValidation deklerasyonlarinin aranacagi assembly'ler
    /// </summary>
    public IEnumerable<Assembly> RegistrationAssemblies { get; set; }

    public IEnumerable<Type> HubList { get; set; }
    public IEnumerable<Type> ApiClientList { get; set; }
}
using Juga.Abstractions.Caching.CacheManagement;
using Juga.Abstractions.Caching;
using Juga.Abstractions.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
namespace Juga.Adf.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AdfServiceCallAttribute : InterceptionAttribute
    {
      
        public AdfServiceCallAttribute(params Type[] clientTypes)
        {
          ClientTypes = clientTypes;  
        }
        public Type[] ClientTypes { get; set; }
    }
}

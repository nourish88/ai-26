using Castle.DynamicProxy;

using Juga.Abstractions.Client;
using Juga.Adf.Client.Attributes;
using Juga.Adf.Client.Models;
using Juga.IoC.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Juga.Adf.Client.Interceptors
{
    public class AdfServiceCallInterceptor : IMethodInterceptor
    {
        private ServiceClientResolver _serviceClientResolver;
        private IUserContextProvider _userContextProvider;
        public AdfServiceCallInterceptor(ServiceClientResolver serviceClientResolver, IUserContextProvider userContextProvider)
        {
            _serviceClientResolver = serviceClientResolver;
            _userContextProvider = userContextProvider;
        }
        public void Intercept(IInvocation invocation)
        {
            var adfAttribute = GetAdfServiceCallAttribute(invocation);
            if (adfAttribute != null)
            {
                var scopes = new List<OperationContextScope>();
                try
                {
                
                    foreach (var clientType in adfAttribute.ClientTypes)
                    {

                        var service = _serviceClientResolver(clientType);
                        OperationContextScope scope = new OperationContextScope(((dynamic)service).InnerChannel);
                        
                        MessageHeader header = MessageHeader.CreateHeader("JUGACustomHeader", "http://JUGA", new MessageHeader<AdfHeader>(new AdfHeader() { UserName = _userContextProvider.ClientName, UserId = Convert.ToInt64(_userContextProvider.ClientId.All(char.IsNumber) ? _userContextProvider.ClientId : "0") }));
                        OperationContext.Current.OutgoingMessageHeaders.Add(header);
                        scopes.Add(scope);  
                    }


                    invocation.Proceed();
                }
                finally
                {
                    foreach (var scope in scopes)
                    {
                        scope.Dispose();    
                    }                
                }
                
            }
            else
            {
                invocation.Proceed();

            }
            return;
        }

        private static AdfServiceCallAttribute GetAdfServiceCallAttribute(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }

            return (AdfServiceCallAttribute)methodInfo.GetCustomAttributes(typeof(AdfServiceCallAttribute), false).FirstOrDefault();
        }


    }

    public delegate ICommunicationObject ServiceClientResolver(Type serviceType);

}

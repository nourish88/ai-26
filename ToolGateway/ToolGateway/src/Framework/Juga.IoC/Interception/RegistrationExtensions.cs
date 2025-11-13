//using Autofac.Builder;
//using Autofac.Core;
//using Autofac.Features.Scanning;
//using Castle.DynamicProxy;
//using System;
//using System.Linq;
//using AD = Autofac.Extras.DynamicProxy;

//namespace Juga.IoC.Interception;

//public static class RegistrationExtensions
//{
//    private static ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions(new ProxyGenerationHook());
        

//    public static IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> EnableClassInterceptors<TLimit, TRegistrationStyle>(
//        this IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> registration)
//    {
//        return AD.RegistrationExtensions.EnableClassInterceptors(registration, ProxyGenerationOptions);
//    }

//    public static IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> EnableClassInterceptors<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(
//        this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration)
//        where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
//    {
//        return AD.RegistrationExtensions.EnableClassInterceptors(registration, ProxyGenerationOptions);
//    }

//    public static IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> EnableInterfaceInterceptors<TLimit, TActivatorData, TSingleRegistrationStyle>(
//        this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration)
//    {
//        return AD.RegistrationExtensions.EnableInterfaceInterceptors(registration, ProxyGenerationOptions);
//    }

//    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InterceptedBy<TLimit, TActivatorData, TStyle>(
//        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
//        params Service[] interceptorServices)
//    {
//        return AD.RegistrationExtensions.InterceptedBy(builder, interceptorServices);
//    }
//    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InterceptedBy<TLimit, TActivatorData, TStyle>(
//        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
//        params string[] interceptorServiceNames)
//    {
//        return AD.RegistrationExtensions.InterceptedBy(builder, interceptorServiceNames);
//    }
//    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InterceptedBy<TLimit, TActivatorData, TStyle>(
//        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
//        params Type[] interceptorServiceTypes)
//    {
//        if (interceptorServiceTypes == null || interceptorServiceTypes.Any(t => t == null))
//        {
//            throw new ArgumentNullException(nameof(interceptorServiceTypes));
//        }

//        return InterceptedBy(builder, interceptorServiceTypes.Select(t => new TypedService(t)).ToArray());
//    }
//}
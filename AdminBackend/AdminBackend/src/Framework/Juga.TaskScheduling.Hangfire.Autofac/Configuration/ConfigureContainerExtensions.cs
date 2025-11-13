//using System;
//using Autofac;
//using Hangfire;

//namespace Juga.TaskScheduling.Hangfire.Autofac.Configuration;

//public static class ConfigureContainerExtensions
//{
//    public static IGlobalConfiguration<AutofacJobActivator> UseAutofacActivator(
//        this IGlobalConfiguration configuration,
//        ILifetimeScope lifetimeScope, bool useTaggedLifetimeScope = true)
//    {
//        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
//        if (lifetimeScope == null) throw new ArgumentNullException(nameof(lifetimeScope));

//        return configuration.UseActivator(new AutofacJobActivator(lifetimeScope, useTaggedLifetimeScope));
//    }
//}
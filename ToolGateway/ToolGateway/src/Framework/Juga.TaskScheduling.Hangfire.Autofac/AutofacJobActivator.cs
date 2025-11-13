//using Autofac;
//using Hangfire;
//using System;

//namespace Juga.TaskScheduling.Hangfire.Autofac
//{
//    public class AutofacJobActivator : JobActivator
//    {
//        public static readonly object LifetimeScopeTag = "BackgroundJobScope";

//        private readonly ILifetimeScope _lifetimeScope;
//        private readonly bool _useTaggedLifetimeScope;

//        public AutofacJobActivator(ILifetimeScope lifetimeScope, bool useTaggedLifetimeScope = true)
//        {
//            if (lifetimeScope == null) throw new ArgumentNullException("lifetimeScope");
//            _lifetimeScope = lifetimeScope;
//            _useTaggedLifetimeScope = useTaggedLifetimeScope;
//        }

//        /// <inheritdoc />
//        public override object ActivateJob(Type jobType)
//        {
//            return _lifetimeScope.Resolve(jobType);
//        }

//        public override JobActivatorScope BeginScope(JobActivatorContext context)
//        {
//            return new AutofacScope(_useTaggedLifetimeScope
//                ? _lifetimeScope.BeginLifetimeScope(LifetimeScopeTag)
//                : _lifetimeScope.BeginLifetimeScope());
//        }

//        class AutofacScope : JobActivatorScope
//        {
//            private readonly ILifetimeScope _lifetimeScope;

//            public AutofacScope(ILifetimeScope lifetimeScope)
//            {
//                _lifetimeScope = lifetimeScope;
//            }

//            public override object Resolve(Type type)
//            {
//                return _lifetimeScope.Resolve(type);
//            }

//            public override void DisposeScope()
//            {
//                _lifetimeScope.Dispose();
//            }
//        }
//    }
//}

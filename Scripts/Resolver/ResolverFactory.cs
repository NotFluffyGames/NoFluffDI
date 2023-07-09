using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public sealed class ResolverFactory<T> : ResolverFactoryFluent<T, ResolverFactory<T>>
    {
        public ResolverFactory(Func<IResolutionContext, T> method) : base(method)
        {
        }
    }
    
    public sealed class ResolverFactory : ResolverFactoryFluent<ResolverFactory>
    {
        public ResolverFactory(Type type, Func<IResolutionContext, object> method, IReadOnlyCollection<Type> extraTypes) 
            : base(type, method, extraTypes)
        {
        }
    }
}
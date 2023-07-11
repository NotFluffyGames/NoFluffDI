using System;

namespace NotFluffy.NoFluffDI
{
    public abstract class ResolverFactoryFluent<T, TFactory> :  BaseResolverFactoryFluent<T, TFactory>
        where TFactory : ResolverFactoryFluent<T, TFactory>
    {
        private readonly ResolveMethod<T> method;

        protected ResolverFactoryFluent(ResolveMethod<T> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public override IAsyncResolver Create()
        {
            return Transient
                ? new TransientResolver(GetIds(), ResolveMethod, GetPostResolveActions(), GetOnDisposeActions())
                : new SingleResolver(GetIds(), ResolveMethod, GetPostResolveActions(), GetOnDisposeActions());
            
            object ResolveMethod(IResolutionContext ctx) => method(ctx);
        }
    }
}
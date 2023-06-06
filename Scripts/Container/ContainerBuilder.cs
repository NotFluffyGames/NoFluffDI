using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseContainerBuilder : IContainerBuilder
    {
        protected List<IResolverFactory> resolvers;
        protected Dictionary<Type, ConverterBind> implicitConverters;
        protected object Context { get; }
        protected IReadOnlyContainer Parent { get; }
        protected event Action<IReadOnlyContainer> BuildCallback;

        protected BaseContainerBuilder(object context = null, IReadOnlyContainer parent = null)
        {
            Parent = parent;
            Context = context;
        }

        public IContainerBuilder Add(IResolverFactory resolverFactory)
        {
            resolvers ??= new List<IResolverFactory>();
            resolvers.Add(resolverFactory);
            return this;
        }

        public IContainerBuilder SetImplicitConverter<TFrom, TTo>(Converter<TFrom, TTo> converter)
        {
            var converterBind = new ConverterBind(typeof(TFrom), ConvertInternal);
            implicitConverters ??= new Dictionary<Type, ConverterBind>();
            implicitConverters.Add(typeof(TTo), converterBind);

            object ConvertInternal(object from) => converter((TFrom)from);
            return this;
        }

        public IReadOnlyContainer Build()
        {
            var container = Create(); 
            
            BuildCallback?.Invoke(container);
            BuildCallback = null;
            
            return container;
        }

        protected abstract IReadOnlyContainer Create();

        public void RegisterBuildCallback(Action<IReadOnlyContainer> callback)
        {
            BuildCallback += callback;
        }
    }
}
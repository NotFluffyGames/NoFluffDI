using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public class ContainerBuilder : IContainerBuilder
    {
        private List<IResolverFactory> resolvers;
        private Dictionary<Type, ConverterBind> implicitConverters;
        private event Action<IReadOnlyContainer> BuildCallback;
        private object Context { get; }
        private IReadOnlyContainer Parent { get; }

        public ContainerBuilder(object context = null, IReadOnlyContainer parent = null)
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
            var container 
                = new Container(
                    Context,
                    resolvers,
                    implicitConverters,
                    Parent);
            
            BuildCallback?.Invoke(container);
            BuildCallback = null;
            
            return container;
        }

        public void RegisterBuildCallback(Action<IReadOnlyContainer> callback)
        {
            BuildCallback += callback;
        }

        private class Container : IReadOnlyContainer
        {
            // private readonly List<IReadOnlyContainer> children = new();
            // public IReadOnlyList<IReadOnlyContainer> Children => children;

            public IReadOnlyContainer Parent { get; private set; }

            public object Context { get; }
            
            public IReadOnlyDictionary<ResolverID, IResolver> Resolvers { get; }
            
            public IReadOnlyDictionary<Type, ConverterBind> ImplicitConverters { get; }

            private bool disposed;

            private readonly Subject<Unit> onDispose = new();
            public IObservable<Unit> OnDispose => onDispose;

            public Container(
                object context, 
                IEnumerable<IResolverFactory> resolvers,
                IReadOnlyDictionary<Type, ConverterBind> implicitConverters,
                IReadOnlyContainer parent = null)
            {
                Context = context;
                ImplicitConverters = implicitConverters;
                Parent = parent;

                Parent?.OnDispose.Subscribe(Dispose);
                
                Resolvers = BuildResolvers(resolvers);
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;
                
                Parent = null;
                onDispose.OnNext(new Unit());
            }
            

            public IContainerBuilder Scope(object context)
            {
                return new ContainerBuilder(context, this);
            }

            public override string ToString() => Context.ToString();

            public bool Contains(Type contract, object id = null)
            {
                var typesToCheck = new Queue<Type>();
                typesToCheck.Enqueue(contract);

                while (typesToCheck.Count > 0)
                {
                    var type = typesToCheck.Dequeue();
                    //First look for a direct resolver
                    if (GetResolver(type, id) != null)
                        return true;

                    var converters = GetAllPossibleConverters(type);

                    //Then look for a implicit converter that his source type can be resolved (BFS search)
                    foreach (var converter in converters)
                        if (converter.Valid)
                            typesToCheck.Enqueue(converter.From);
                }

                return false;
            }
            
            private IReadOnlyDictionary<ResolverID, IResolver> BuildResolvers(IEnumerable<IResolverFactory> factories)
            {
                if (factories == null)
                    return null;
                
                var resolvers = new Dictionary<ResolverID, IResolver>();
                
                var toResolve = new Queue<IResolver>();
                foreach (var factory in factories)
                {
                    var resolver = factory.Create();

                    foreach (var id in resolver.IDs)
                        resolvers[id] = resolver;

                    if (!factory.IsLazy)
                        toResolve.Enqueue(resolver);
                }

                while (toResolve.Count > 0)
                {
                    var resolver = toResolve.Dequeue();
                    var ctx = new ResolutionContext(resolver, this, this);
                    resolver.Resolve(ctx);
                }

                return resolvers;
            }

            public object Resolve(Type contract, object id = null)
                => Resolve(new ResolverID(contract, id));

            private object Resolve(ResolverID resolverID)
            {
                var contract = resolverID.Type;
                var id = resolverID.Id;
                //Try resolve using a direct resolver
                var ctx = GetResolver(resolverID);

                if (ctx != null)
                    return ctx.Resolve();

                //Try to find a matching converters and resolve their parent type
                foreach (var converter in GetAllPossibleConverters(contract))
                {
                    if (!converter.Valid)
                        continue;

                    resolverID = new ResolverID(converter.From, id);
                    var converterCtx = GetResolver(resolverID);
                    if (converterCtx != null)
                        return converter.Converter(converterCtx.Resolve());
                }

                throw new NoMatchingResolverException(contract);
            }

            public ConverterBind GetConverter(Type from, Type to)
            {
                return GetAllPossibleConverters(to)
                    .FirstOrDefault(c => c.From == from);
            }

            private IEnumerable<ConverterBind> GetAllPossibleConverters(Type to)
            {
                return SelfAndParents().SelectWhile<IReadOnlyContainer, ConverterBind>(TryGetConverter);

                bool TryGetConverter(IReadOnlyContainer node, out ConverterBind c)
                {
                    if (node.ImplicitConverters == null)
                    {
                        c = default;
                        return false;
                    }
                    
                    return node.ImplicitConverters.TryGetValue(to, out c);
                }
            }

            private IResolutionContext GetResolver(Type contract, object id = null)
                => GetResolver(new ResolverID(contract, id));

            private IResolutionContext GetResolver(ResolverID resolverId)
            {
                //Try resolve using a direct resolver
                return SelfAndParents()
                    .SelectWhile<IReadOnlyContainer, IResolutionContext>(TryGetResolver)
                    .FirstOrDefault();

                bool TryGetResolver(IReadOnlyContainer node, out IResolutionContext c)
                {
                    if (node.Resolvers == null)
                    {
                        c = default;
                        return false;
                    }
                    
                    if (node.Resolvers.TryGetValue(resolverId, out var r))
                    {
                        c = new ResolutionContext(r, node, this);
                        return true;
                    }

                    c = default;
                    return false;
                }
            }

            private IEnumerable<IReadOnlyContainer> SelfAndParents()
            {
                IReadOnlyContainer current = this;

                while (current != null)
                {
                    yield return current;
                    current = current.Parent;
                }
            }
        }
    }
}
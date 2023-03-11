using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public class Container : IContainer
    {
        private readonly List<IContainer> children = new();
        public IReadOnlyList<IReadOnlyContainer> Children => children;

        public IReadOnlyContainer Parent { get; private set; }

        public object Context { get; }

        private readonly Dictionary<ResolverID, IResolver> resolvers = new();
        public IReadOnlyDictionary<ResolverID, IResolver> Resolvers => resolvers;

        private readonly Dictionary<Type, ConverterBind> implicitConverters = new();
        public IReadOnlyDictionary<Type, ConverterBind> ImplicitConverters => implicitConverters;

        private bool disposed;

        private readonly Subject<Unit> onDispose = new();
        public IObservable<Unit> OnDispose => onDispose;
        
        public Container(object context, IReadOnlyContainer parent = null)
        {
            Context = context;
            Parent = parent;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            foreach (var child in children.Reversed())
                child.Dispose();

            children.Clear();
            Parent = null;
            onDispose.OnNext(new Unit());
        }

        public IContainer Scope(object context)
        {
            var scoped = new Container(context, this);
            children.Add(scoped);

            scoped.OnDispose.Subscribe(OnDispose);

            return scoped;

            void OnDispose() => children.Remove(scoped);
        }

        public void Install(IEnumerable<IResolverFactory> resolverFactories)
        {
            if (resolverFactories != null)
                AddResolvers(resolverFactories);
        }

        private void AddResolvers(IEnumerable<IResolverFactory> factories)
        {
            factories = factories as ICollection<IResolverFactory> ?? factories.ToArray();

            var toResolve = new Queue<IResolver>();
            foreach (var factory in factories)
            {
                var factoryResolvers = factory.Create();

                AddResolver(factoryResolvers);

                if (!factory.IsLazy)
                    toResolve.Enqueue(factoryResolvers);
            }

            while (toResolve.Count > 0)
            {
                var resolver = toResolve.Dequeue();
                var ctx = new ResolutionContext(resolver, this, this);
                resolver.Resolve(ctx);
            }
        }

        private void AddResolver(IResolver resolverToAdd)
        {
            foreach (var id in resolverToAdd.IDs)
                resolvers[id] = resolverToAdd;
        }

        public override string ToString() => Context.ToString();

        public void SetImplicitConverter<TFrom, TTo>(Converter<TFrom, TTo> converter)
        {
            var converterBind = new ConverterBind(typeof(TFrom), ConvertInternal);
            implicitConverters.Add(typeof(TTo), converterBind);

            object ConvertInternal(object from) => converter((TFrom) from);
        }

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
                if(!converter.Valid)
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
            return this.SelfAndParents().SelectWhile<IReadOnlyContainer, ConverterBind>(TryGetConverter);

            bool TryGetConverter(IReadOnlyContainer node, out ConverterBind c)
            {
                return node.ImplicitConverters.TryGetValue(to, out c);
            }
        }

        private IResolutionContext GetResolver(Type contract, object id = null)
            => GetResolver(new ResolverID(contract, id));
        
        private IResolutionContext GetResolver(ResolverID resolverId)
        {
            //Try resolve using a direct resolver
            return this.SelfAndParents()
                .SelectWhile<IReadOnlyContainer, IResolutionContext>(TryGetResolver)    
                .FirstOrDefault();

            bool TryGetResolver(IReadOnlyContainer node, out IResolutionContext c)
            {
                if (node.Resolvers.TryGetValue(resolverId, out var r))
                {
                    c = new ResolutionContext(r, node, this);
                    return true;
                }

                c = default;
                return false;
            }
        }
    }
}
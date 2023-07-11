using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Cysharp.Threading.Tasks;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public class ContainerBuilder : BaseContainerBuilder
    {
        public ContainerBuilder(object context = null, IReadOnlyContainer parent = null)
            : base(context, parent)
        {
        }

        protected override IContainerBuildResult Create()
        {
            AssertNotDisposed();

            var container = new Container(
                Context,
                Parent,
                resolverFactories);

            return new ContainerBuildResult(container, container);
        }

        private class Container : IReadOnlyContainer, IReactiveDisposable
        {
            public object Context { get; }

            public IReadOnlyDictionary<ResolverID, IAsyncResolver> Resolvers { get; }

            private bool disposed;

            private readonly HashSet<ResolverID> _currentlyResolved = new();

            private readonly Subject<Unit> onDispose = new();
            public IObservable<Unit> OnDispose => onDispose;

            public Container(
                object context,
                IReadOnlyContainer parent,
                IEnumerable<IResolverFactory> factories)
            {
                Context = context;

                var resolversDict = parent == null 
                    ? new Dictionary<ResolverID, IAsyncResolver>()
                    : new Dictionary<ResolverID, IAsyncResolver>(parent.Resolvers);
                
                Resolvers = resolversDict;

                if(factories != null)
                    foreach (var resolver in BuildResolvers(factories))
                        if(resolver is { IDs: not null })
                            foreach (var id in resolver.IDs)
                                resolversDict[id] = resolver;

                var hashset = new HashSet<IAsyncResolver>();

                foreach (var resolver in resolversDict.Values)
                    hashset.Add(resolver);

                //Disposed on container dispose
                var composite = new CompositeDisposable(hashset.Count).AddTo(this);
                foreach (var resolver in hashset)
                    resolver.TakeRefCountToken().AddTo(composite);
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;
                
                onDispose.OnNext();
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

                    var resolverID = new ResolverID(type, id);
                    //First look for a direct resolver
                    if (GetResolver(resolverID) != null)
                        return true;
                }

                return false;
            }

            private static IEnumerable<IAsyncResolver> BuildResolvers(IEnumerable<IResolverFactory> factories)
            {
                if (factories == null)
                    yield break;
                
                foreach (var factory in factories)
                {
                    var resolver = factory?.Create();

                    if (resolver == null)
                        continue;
                    
                    yield return resolver;
                }
            }
            
            public object Resolve(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var asyncResolver = GetResolver(resolverID);

                if (asyncResolver is not IResolver resolver)
                    throw new NoMatchingResolverException(contract);
                
                if (!_currentlyResolved.Add(resolverID))
                    throw new CircularDependencyException(resolverID);
                
                var value = resolver.Resolve(new ResolutionContext(this)); 

                _currentlyResolved.Remove(resolverID);
                
                return value;
            }
            
            public async UniTask<object> ResolveAsync(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var resolver = GetResolver(resolverID);

                if (resolver == null)
                    throw new NoMatchingResolverException(contract);

                if (!_currentlyResolved.Add(resolverID))
                    throw new CircularDependencyException(resolverID);
                
                var value = await resolver.ResolveAsync(new ResolutionContext(this)); 

                _currentlyResolved.Remove(resolverID);
                
                return value;
            }

            private IAsyncResolver GetResolver(ResolverID resolverId) 
                => Resolvers.TryGetValue(resolverId, out var r) ? r : null;

            ~Container()
            {
                Dispose();
            }
        }
    }
}
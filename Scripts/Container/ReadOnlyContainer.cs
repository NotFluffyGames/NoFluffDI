//using NotFluffy.NoFluffRx;
//using System;
//using System.Collections.Generic;
//using System.Reactive;
//using System.Reactive.Subjects;

//namespace NotFluffy.NoFluffDI
//{
//    public class ReadOnlyContainer : IReadOnlyContainer
//    {
//        public object Context { get; }

//        public IReadOnlyDictionary<ResolverID, IResolver> Resolvers { get; }

//        public IReadOnlyContainer Parent { get; }

//        private List<IReadOnlyContainer> children = new ();
//        public IReadOnlyList<IReadOnlyContainer> Children => children;

//        private Subject<Unit> _onDispose = new();

//        public ReadOnlyContainer(object context, IReadOnlyDictionary<ResolverID, IResolver> resolvers, IReadOnlyContainer parent)
//        {
//            Context = context;
//            Resolvers = resolvers ?? throw new NullReferenceException(nameof(resolvers));
//            Parent = parent;
//        }

//        public IObservable<Unit> OnDispose => _onDispose;

//        public IContainer Scope(object context)
//        {
//            var scoped = new Container(this, context);
//            children.Add(scoped);

//            scoped.OnDispose.Subscribe(OnDispose);

//            return scoped;

//            void OnDispose() => children.Remove(scoped);
//        }

//        public bool TryGetResolver(Type contract, out IResolutionContext ctx, object id = null)
//        {
//            var resID = new ResolverID(contract, id);

//            if (Resolvers.TryGetValue(resID, out var resolver))
//            {
//                ctx = new ResolutionContext(resolver, this, this);
//                return true;
//            }

//            if (Parent != null && Parent.TryGetResolver(contract, out ctx, id))
//            {
//                ctx = new ResolutionContext(ctx.Resolver, ctx.OriginContainer, this);
//                return true;
//            }

//            ctx = default;
//            return false;
//        }

//        public bool Contains(Type contract, object id = null)
//        {
//            return TryGetResolver(contract, out _, id);
//        }

//        public object Resolve(Type contract, object id = null)
//        {
//            var key = new ResolverID(contract, id);
//            if (TreeExt.TryRecursiveDo<IReadOnlyContainer, object>(this, ResolveRecursive, out var result))
//                return result;

//            bool ResolveRecursive(IReadOnlyContainer container, out object result)
//            {
//                if(container.Re)
//            }
//        }

//        public bool CanConvert(Type from, object id = null)
//        {
//            throw new NotImplementedException();
//        }

//        public TTo Convert<TFrom, TTo>(TFrom from)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
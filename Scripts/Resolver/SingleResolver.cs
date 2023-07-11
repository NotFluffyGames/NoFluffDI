using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public class SingleResolver : TransientResolver
    {
        public SingleResolver(
            IEnumerable<ResolverID> ids, 
            ResolveMethod method, 
            IEnumerable<PostResolveAction> postResolveActions,
            IEnumerable<OnDisposeAction> postDisposeActions)
            : base(ids, method, postResolveActions, postDisposeActions)
        {
        }

        protected override int ResolvedObjectsCapacity => 1;

        private object instance;

        public override object Resolve(IResolutionContext container)
        {
            if (instance == null)
                return instance = base.Resolve(container);

            IncrementResolveCount();
            return instance;
        }
    }
}
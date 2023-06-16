using System.Collections.Generic;
using Benchmark.NestedModel;
using Benchmark.Utilities;

namespace NotFluffy.NoFluffDI.Benchmark
{
    public class NestedBenchmarkNoFluffDI : MonoProfiler
    {
        private readonly IContainerBuilder _builder = new ContainerBuilder(string.Empty);
        private IReadOnlyContainer _container;

        private void Start()
        {
            _builder.Add(Resolvers());
            _container = _builder.Build().Container;
        }

        private static IEnumerable<IResolverFactory> Resolvers()
        {
            yield return Resolve.FromMethodAsync<IA>(async c => new A( await c.Resolve<IB>())).AsTransient();
            yield return Resolve.FromMethodAsync<IB>(async c => new B( await c.Resolve<IC>())).AsTransient();
            yield return Resolve.FromMethodAsync<IC>(async c => new C( await c.Resolve<ID>())).AsTransient();
            yield return Resolve.FromMethodAsync<ID>(async c => new D( await c.Resolve<IE>())).AsTransient();
            yield return Resolve.FromNew<IE, E>().AsTransient();
        }

        protected override int Order => 0;

        protected override void Sample() => _container.Resolve<IA>();
    }
}
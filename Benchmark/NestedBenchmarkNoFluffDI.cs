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
            yield return Resolve.FromMethod<IA>(c => new A(c.Resolve<IB>())).AsTransient();
            yield return Resolve.FromMethod<IB>(c => new B(c.Resolve<IC>())).AsTransient();
            yield return Resolve.FromMethod<IC>(c => new C(c.Resolve<ID>())).AsTransient();
            yield return Resolve.FromMethod<ID>(c => new D(c.Resolve<IE>())).AsTransient();
            yield return Resolve.FromNew<IE, E>().AsTransient();
        }

        protected override int Order => 0;

        protected override void Sample() => _container.Resolve<IA>();
    }
}
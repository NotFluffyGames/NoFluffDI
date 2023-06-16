using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ContainerBuildResult : IContainerBuildResult
    {
        public ContainerBuildResult(IReadOnlyContainer container, IDisposable containerDisposable)
        {
            Container = container;
            ContainerDisposable = containerDisposable;
        }

        public IReadOnlyContainer Container { get; }
        public IDisposable ContainerDisposable { get; }
    }
}
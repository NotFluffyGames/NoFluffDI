using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ResolutionContext : IResolutionContext
    {
        public ResolutionContext(IReadOnlyContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }
        
        public IReadOnlyContainer Container { get; }
    }
}
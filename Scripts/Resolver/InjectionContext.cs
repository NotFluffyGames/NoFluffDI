using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ResolutionContext : IResolutionContext
    {
        public ResolutionContext(
            IReadOnlyContainer resolverOriginContainer,
            IReadOnlyContainer currentResolutionContainer)
        {
            OriginContainer = resolverOriginContainer ?? throw new ArgumentNullException(nameof(resolverOriginContainer));
            Container = currentResolutionContainer ?? throw new ArgumentNullException(nameof(currentResolutionContainer));
        }
        
        public IReadOnlyContainer OriginContainer { get; }
        public IReadOnlyContainer Container { get; }
    }
}
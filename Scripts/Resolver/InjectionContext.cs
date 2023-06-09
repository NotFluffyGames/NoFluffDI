using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ResolutionContext : IResolutionContext
    {
        public ResolutionContext( IResolver resolver, IReadOnlyContainer resolverOriginContainer,
            IReadOnlyContainer currentResolutionContainer)
        {
            ContextResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            OriginContainer = resolverOriginContainer ??
                                      throw new ArgumentNullException(nameof(resolverOriginContainer));
            Container = currentResolutionContainer ??
                                         throw new ArgumentNullException(nameof(currentResolutionContainer));
        }
        
        public IResolver ContextResolver { get; }
        public IReadOnlyContainer OriginContainer { get; }
        public IReadOnlyContainer Container { get; }
    }
}
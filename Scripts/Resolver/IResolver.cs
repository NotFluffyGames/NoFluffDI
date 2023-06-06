using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
	public interface IResolutionContext
	{
		IResolver ContextResolver { get; }
		IReadOnlyContainer OriginContainer { get; }
		IReadOnlyContainer CurrentContainer { get; }
	}
    public interface IResolver
	{
		IEnumerable<ResolverID> IDs { get; }
		int Resolutions { get; }
		object Resolve(IResolutionContext context);
	}

    public interface IResolverFactory
    {
	    bool IsLazy { get; }
	    IResolver Create();
    }
}
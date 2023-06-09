using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
	public interface IResolutionContext
	{
		IResolver ContextResolver { get; }
		IReadOnlyContainer OriginContainer { get; }
		IReadOnlyContainer Container { get; }
	}
    public interface IResolver
	{
		IEnumerable<ResolverID> IDs { get; }
		int Resolutions { get; }
		UniTask<object> Resolve(IResolutionContext context);
	}

    public interface IResolverFactory
    {
	    bool IsLazy { get; }
	    IResolver Create();
    }
}
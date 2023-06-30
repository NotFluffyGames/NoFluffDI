using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
	/// <summary>
	/// Invoked after each new instance is created
	/// </summary>
	public delegate UniTask AsyncPostResolveAction(object resolved, IResolutionContext context);
	public delegate void PostResolveAction(object resolved, IResolutionContext context);
	
	public interface IResolutionContext
	{
		IReadOnlyContainer OriginContainer { get; }
		IReadOnlyContainer Container { get; }
	}
	
	public interface IAsyncResolver
	{
		IEnumerable<ResolverID> IDs { get; }
		int Resolutions { get; }
		UniTask<object> ResolveAsync(IResolutionContext context);
	}
	
	public interface IResolver : IAsyncResolver
	{
		object Resolve(IResolutionContext context);
	}

    public interface IResolverFactory
    {
	    bool IsLazy { get; }
	    IAsyncResolver Create();
    }
}
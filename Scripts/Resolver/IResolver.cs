using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
	public interface IAsyncResolver : IDisposable
	{
		IReadOnlyList<ResolverID> IDs { get; }
		int Resolutions { get; }
		UniTask<object> ResolveAsync(IResolutionContext context);
		
		IDisposable TakeRefCountToken();
	}
	
	public interface IResolver : IAsyncResolver
	{
		object Resolve(IResolutionContext context);
	}
}
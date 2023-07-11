using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    /// <summary>
    /// Invoked after each new instance is created
    /// </summary>
    public delegate UniTask AsyncPostResolveAction(object resolved, IResolutionContext context);
    /// <summary>
    /// Invoked after each new instance is created
    /// </summary>
    public delegate UniTask AsyncPostResolveAction<in T>(T resolved, IResolutionContext context);
	
    public delegate void OnDisposeAction(object disposed);
    public delegate void OnDisposeAction<in T>(T disposed);
	
    public delegate void PostResolveAction(object resolved, IResolutionContext context);
    public delegate void PostResolveAction<in T>(T resolved, IResolutionContext context);
	
    public delegate object ResolveMethod(IResolutionContext context);
    public delegate UniTask<object> AsyncResolveMethod(IResolutionContext context);
    public delegate T ResolveMethod<out T>(IResolutionContext context);
    public delegate UniTask<T> AsyncResolveMethod<T>(IResolutionContext context);
}
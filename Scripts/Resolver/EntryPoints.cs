using System.Threading;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    /// <summary>
    /// Only possible with async resolvers, similar to IStartable but waits until Initialize finishes before resolver is ready 
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Only possible with async resolvers, waits until Initialize finishes before resolver is ready 
        /// </summary>
        /// ///
        /// <param name="context">The resolution context used to resolve the object</param>
        /// <param name="resolverDisposed">Disposed when the object's resolver is disposed</param>
        UniTask Initialize(IResolutionContext context, CancellationToken resolverDisposed);
    }
    
    /// <summary>
    /// Called after the object was created 
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// Called after the object was created 
        /// </summary>
        /// <param name="context">The resolution context used to resolve the object</param>
        /// <param name="resolverDisposed">Disposed when the object's resolver is disposed</param>
        UniTaskVoid StartAsync(IResolutionContext context, CancellationToken resolverDisposed);
    }

    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}
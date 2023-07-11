using System;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI.Unity.Unity.EntryPoints.Tickable
{
    public static class TickableExt
    {
        public static TFactory BindAsTickable<T, TFactory>(this TFactory factory)
            where TFactory : IResolverFactory<T>
            where T : ITickable
        {
            factory.AddPostResolveAction(OnResolve);
            return factory;

            void OnResolve(T resolved, IResolutionContext context)
            {
                factory.ChainDisposable(resolved.RegisterTickable());
            }
        }


        private static IDisposable RegisterTickable(this ITickable tickable)
        {
            var playerLoopItem = new TickablePlayerLoop(tickable);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, playerLoopItem);
            return playerLoopItem;
        }
    }
}
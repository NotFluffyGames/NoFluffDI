using System;
using System.Reactive;
using Cysharp.Threading.Tasks;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.NoFluffDI
{
    public interface IInjectContext
    {
        /// <summary>
        /// The injecting container
        /// </summary>
        IReadOnlyContainer Container { get; }
        
        /// <summary>
        /// Called when all injectables finished injection
        /// </summary>
        void RegisterInjectCallback(Action callback);
    }
    
    public interface IInjectable
    {
        UniTask Inject(IInjectContext injectContext);
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity
{
    public abstract class InjectableScriptableObject : ScriptableObject, IInjectable
    {
        public abstract UniTask Inject(IInjectContext injectContext);
    }
}
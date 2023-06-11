using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity
{
    public abstract class InjectableComponent : MonoBehaviour, IInjectable
    {
        public abstract UniTask Inject(IInjectContext injectContext);
    }
}
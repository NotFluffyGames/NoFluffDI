using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class InjectContext : IInjectContext
    {
        public IReadOnlyContainer Container { get; }

        private event Action OnComplete;

        public UniTask Inject(IEnumerable<IInjectable> injectables)
        {
            return Inject(injectables.WhereNotNull().Select<IInjectable, Inject>(i => i.Inject));
        }
        
        public async UniTask Inject(IEnumerable<Inject> injectables)
        {
            await UniTask.WhenAll(injectables.Select(injectable => injectable.Invoke(this)));
            InjectionComplete();
        }

        public void InjectionComplete()
        {
            OnComplete?.Invoke();
            OnComplete = null;
        }
        
        public void RegisterInjectCallback(Action callback)
        {
            OnComplete += callback;
        }

        public InjectContext(IReadOnlyContainer container)
        {
            Container = container;
        }
    }
}
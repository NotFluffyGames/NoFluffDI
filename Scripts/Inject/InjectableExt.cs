using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.NoFluffDI
{
    public static class InjectableExt
    {
        public static UniTask Inject(this IEnumerable<IInjectable> injectables, IInjectContext injectContext)
        {
            return UniTask.WhenAll(injectables.Where(i => i != null).Select(SelectInject));
            
            UniTask SelectInject(IInjectable injectable) => injectable.Inject(injectContext);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    [System.Serializable]
    public class InstallersCollection : IEnumerable<IInstallable>, IInstallable
    {
        [SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();
        [SerializeField] private List<ScriptableInstaller> _scriptableInstallers = new List<ScriptableInstaller>();
        
        private IEnumerable<IInstallable> Installers
        {
            get
            {
                foreach (var installer in _monoInstallers)
                    if(installer != null)
                        yield return installer;

                foreach (var installer in _scriptableInstallers)
                    if(installer != null)
                        yield return installer;
            }
        }

        public IEnumerator<IInstallable> GetEnumerator() => Installers.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => Installers.GetEnumerator();

        public IEnumerable<IResolverFactory> GetBindings(IReadOnlyContainer container)
            => Installers.SelectMany(i => i.GetBindings(container));
    }
}
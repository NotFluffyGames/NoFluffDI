using System;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.NoFluffDI
{
    public sealed class ProjectContext : ScriptableObject
    {
        public const string ProjectContextPath = "ProjectContext";
        
        private static IReadOnlyContainer _instance;
        public static IReadOnlyContainer Instance => _instance ??= LoadContainer();

        private static IReadOnlyContainer LoadContainer()
        {
            var projectContext = Resources.Load<ProjectContext>(ProjectContextPath);

            var buildResult = projectContext._installers.BuildContainer("ProjectContainer");
            
            _containerDisposable = buildResult.ContainerDisposable;

            Application.quitting += ApplicationOnQuitting;

            return buildResult.Container;
        }

        private static IDisposable _containerDisposable;
        
        [SerializeField] private InstallersCollection _installers;


        private static void ApplicationOnQuitting()
        {
            DisposeProjectContext();
        }

        public static void DisposeProjectContext()
        {
            _instance = null;
            _containerDisposable?.Dispose();
            _containerDisposable = null;
        }

#if UNITY_EDITOR
        //For projects with Domain reloading disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForDomainReloadingDisabled()
        {
            DisposeProjectContext();
        }
#endif

    }
}
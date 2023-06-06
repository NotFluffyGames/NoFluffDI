using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public class ProjectContext : ScriptableObject
    {
        public const string ProjectContextPath = "ProjectContext";
        public static IReadOnlyContainer Instance;

#if UNITY_EDITOR
        //For projects with Domain reloading disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForDomainReloadingDisabled()
        {
            //Instance.Dispose();
            Instance = null;
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void BeforeAwakeOfFirstSceneOnly() => ValidateInitialized();

        public static void ValidateInitialized()
        {
            if (IsInitialized)
                return;

            if (ResourcesUtilities.TryLoad<ProjectContext>(ProjectContextPath, out var projectContext))
                projectContext.Init();

            IsInitialized = true;
        }

        [SerializeField]
        protected InstallersCollection installers;
        
        public static bool IsInitialized { get; private set; }

        private void Init()
        {
            Instance = installers.CreateContainer("ProjectContainer");
            
            Application.quitting += ApplicationOnQuitting;
        }

        private static void ApplicationOnQuitting()
        {
            //Instance.Dispose();
            Instance = null;
        }
    }
}
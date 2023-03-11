using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public class ProjectContext : ScriptableObject
    {
        public const string ProjectContextPath = "ProjectContext";

#if UNITY_EDITOR
        //For projects with Domain reloading disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForDomainReloadingDisabled() => ProjectContainer.Instance.Dispose();
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
            ProjectContainer.Instance.Install(installers);
            
            Application.quitting += ApplicationOnQuitting;
        }

        private static void ApplicationOnQuitting()
        {
            ProjectContainer.Instance.Dispose();
        }
    }
}
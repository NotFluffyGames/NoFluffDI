using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NotFluffy.NoFluffDI
{
    public static class SceneContextExt
    {
        private static readonly Dictionary<Scene, IReadOnlyContainer> sceneContainers = new();
        private static readonly List<SceneLazyBind> lazyBinds = new();
        private static bool resolvingLazyBinds;

#if UNITY_EDITOR
        //For projects with Domain reloading disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForDomainReloadingDisabled()
        {
            sceneContainers.Clear();
            resolvingLazyBinds = false;
        }
#endif

        static SceneContextExt()
        {
            SceneManager.sceneUnloaded += OnUnload;
        }
        private static void OnUnload(Scene scene)
        {
            // if (sceneContainers.TryGetValue(scene, out var container))
            //     container.Dispose();
        }


        public static IReadOnlyContainer GetContainer(this Scene scene)
        {
            InitLazyBinds();

            if (sceneContainers.Count > 0 && sceneContainers.TryGetValue(scene, out var container))
                return container;

            return ProjectContext.Instance;
        }
        private static void InitLazyBinds()
        {
            if (resolvingLazyBinds || lazyBinds.Count == 0)
                return;

            resolvingLazyBinds = true;

            var sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var bindIndex = lazyBinds.FindIndex(b => b.ScenePredicate(scene));

                if (bindIndex != -1)
                {
                    scene.BindContainer(lazyBinds[bindIndex].Container);
                    lazyBinds.RemoveAt(bindIndex);
                }
            }

            resolvingLazyBinds = false;
        }

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IContainerBuilder CreateScope(this Scene scene)
        {
            if (scene.TryGetScope(out _))
                throw new Exception($"Scope already created for scene {scene}");

            return ContextHelper.CreateScope(scene.GetContainer(), scene, scene, sceneContainers);
        }
        
        public static bool TryGetScope(this Scene scene, out IReadOnlyContainer container)
        {
            return sceneContainers.TryGetValue(scene, out container);
        }

        private static void BindContainer(this Scene scene, IReadOnlyContainer container)
            => ContextHelper.BindContainer(scene, sceneContainers, container);


        public static AsyncOperation LoadSceneAsync(this IReadOnlyContainer container, int sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            container.LazyBindContainer(s => s.buildIndex == sceneIndex);
            return SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        }

        public static AsyncOperation LoadSceneAsync(this IReadOnlyContainer container, string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            container.LazyBindContainer(s => s.name == sceneName);
            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        }
        
        private static void LazyBindContainer(this IReadOnlyContainer container, Predicate<Scene> scenePredicate)
        {
            lazyBinds.Add(new SceneLazyBind(scenePredicate, container));
            InitLazyBinds();
        }

        private readonly struct SceneLazyBind
        {
            public readonly Predicate<Scene> ScenePredicate;
            public readonly IReadOnlyContainer Container;

            public SceneLazyBind(Predicate<Scene> scenePredicate, IReadOnlyContainer container)
            {
                ScenePredicate = scenePredicate;
                Container = container;
            }
        }
    }
}
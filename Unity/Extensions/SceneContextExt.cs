using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NotFluffy.NoFluffDI
{
    public static class SceneContextExt
    {
        private static readonly Dictionary<Scene, IContainer> sceneContainers = new();
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
            if (sceneContainers.TryGetValue(scene, out var container))
                container.Dispose();
        }


        public static IReadOnlyContainer GetContainer(this Scene scene)
        {
            InitLazyBinds();

            if (sceneContainers.Count > 0 && sceneContainers.TryGetValue(scene, out var container))
                return container;

            return ProjectContainer.Instance;
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
        public static IContainer GetOrCreateScope(this Scene scene)
        {
            return ContextHelper.GetOrCreateScope(
                scene.GetContainer(),
                scene,
                scene,
                sceneContainers);
        }

        private static void BindContainer(this Scene scene, IContainer container)
            => ContextHelper.BindContainer(scene, sceneContainers, container);

        private static void LazyBindContainer(this IContainer container, Predicate<Scene> scenePredicate)
        {
            lazyBinds.Add(new SceneLazyBind(scenePredicate, container));
            InitLazyBinds();
        }

        public static AsyncOperation LoadSceneAsync(this IContainer container, int sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            container.LazyBindContainer((s) => s.buildIndex == sceneIndex);
            return SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        }

        public static AsyncOperation LoadSceneAsync(this IContainer container, string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            container.LazyBindContainer((s) => s.name == sceneName);
            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        }

        private readonly struct SceneLazyBind
        {
            public readonly Predicate<Scene> ScenePredicate;
            public readonly IContainer Container;

            public SceneLazyBind(Predicate<Scene> scenePredicate, IContainer container)
            {
                ScenePredicate = scenePredicate;
                Container = container;
            }
        }
    }
}
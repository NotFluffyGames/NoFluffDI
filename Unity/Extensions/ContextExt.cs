using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NotFluffy.NoFluffDI
{
    public static class ContextExt
    {
        #region Instantiate

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static GameObject InstantiateFromPrefab(
            this GameObject gameObject,
            GameObject prefab,
            Transform parent = null)
            => gameObject
                .GetContainer()
                .InstantiateFromPrefab(prefab, parent);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static T InstantiateFromPrefab<T>(
            this GameObject gameObject,
            T prefab,
            Transform parent = null)
            where T : Component
            => gameObject
                .GetContainer()
                .InstantiateFromPrefab(prefab, parent);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static GameObject InstantiateFromPrefab(
            this Component component,
            GameObject prefab,
            Transform parent = null)
            => component
                .GetContainer()
                .InstantiateFromPrefab(prefab, parent);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static T InstantiateFromPrefab<T>(
            this Component component,
            T prefab,
            Transform parent = null)
            where T : Component
            => component
                .GetContainer()
                .InstantiateFromPrefab(prefab, parent);

        #endregion

        #region LoadSceneAsync

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static AsyncOperation LoadSceneAsync(
            this GameObject gameObject,
            int sceneIndex,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => gameObject
                .GetContainer()
                .LoadSceneAsync(sceneIndex, loadSceneMode);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static AsyncOperation LoadSceneAsync(
            this GameObject gameObject,
            string sceneName,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => gameObject
                .GetContainer()
                .LoadSceneAsync(sceneName, loadSceneMode);
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static AsyncOperation LoadSceneAsync(
            this Component component,
            int sceneIndex,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => component
                .GetContainer()
                .LoadSceneAsync(sceneIndex, loadSceneMode);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static AsyncOperation LoadSceneAsync(
            this Component component,
            string sceneName,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            => component
                .GetContainer()
                .LoadSceneAsync(sceneName, loadSceneMode);

        #endregion

        #region GetContainer

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static IReadOnlyContainer GetContainer(this GameObject gameObject)
            => gameObject.transform.GetContainer();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static IReadOnlyContainer GetContainer(this Component component)
            => component.transform.GetContainer();

        #endregion

        #region Resolve

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static T Resolve<T>(this Scene scene, object id = null)
            => scene.GetContainer().Resolve<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static T Resolve<T>(this GameObject gameObject, object id = null)
            => gameObject.transform.GetContainer().Resolve<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static T Resolve<T>(this Component component, object id = null)
            => component.GetContainer().Resolve<T>(id);
        
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static UniTask<T> ResolveAsync<T>(this Scene scene, object id = null)
                => scene.GetContainer().ResolveAsync<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static UniTask<T> ResolveAsync<T>(this GameObject gameObject, object id = null)
                => gameObject.transform.GetContainer().ResolveAsync<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static UniTask<T> ResolveAsync<T>(this Component component, object id = null)
                => component.GetContainer().ResolveAsync<T>(id);

        #endregion

        #region TryGetScope

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static bool TryGetScope(this GameObject gameObject, out IReadOnlyContainer container)
            => gameObject.transform.TryGetScope(out container);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static bool TryGetScope(this Component component, out IReadOnlyContainer container)
            => component.transform.TryGetScope(out container);

        #endregion

        #region CreateScope

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static IContainerBuilder CreateScope(this GameObject gameObject)
            => gameObject.transform.CreateScope();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UsedImplicitly]
        public static IContainerBuilder CreateScope(this Component component)
            => component.transform.CreateScope();

        #endregion
    }
}
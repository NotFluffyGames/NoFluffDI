using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NotFluffy.NoFluffDI
{
    public static class ContextExt
    {
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IReadOnlyContainer GetContainer(this GameObject gameObject) => gameObject.transform.GetContainer();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IReadOnlyContainer GetContainer(this Component component) => component.transform.GetContainer();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static T Resolve<T>(this Scene scene, object id = null)
            => scene.GetContainer().Resolve<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static T Resolve<T>(this GameObject gameObject, object id = null)
            => gameObject.transform.GetContainer().Resolve<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static T Resolve<T>(this Component component, object id = null)
            => component.GetContainer().Resolve<T>(id);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IContainer GetOrCreateScope(this GameObject gameObject)
            => gameObject.transform.GetOrCreateScope();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IContainer GetOrCreateScope(this Component component)
            => component.transform.GetOrCreateScope();

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void LazyGetOrCreateScope(this GameObject gameObject, Action<IContainer> callback)
            => gameObject.transform.LazyGetOrCreateScope(callback);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void LazyGetOrCreateScope(this Component component, Action<IContainer> callback)
            => component.transform.LazyGetOrCreateScope(callback);
    }
}
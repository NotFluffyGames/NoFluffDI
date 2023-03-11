using System;
using System.Collections.Generic;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public static class GameobjectContextExt
    {
        private static readonly Dictionary<Transform, IContainer> gameObjectContainers = new();
        private static readonly List<GameObjectLazyBind> LazyBinds = new();

#if UNITY_EDITOR
        //For projects with Domain reloading disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForDomainReloadingDisabled()
        {
            gameObjectContainers.Clear();
            LazyBinds.Clear();
        }
#endif

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IReadOnlyContainer GetContainer(this Transform transform)
        {
            InitLazyBinds();

            //Turn into multi-key dictionary with scene key?
            var containers = gameObjectContainers;

            if (gameObjectContainers.Count > 0)
            {
                var current = transform;

                while (current != null)
                {
                    if (containers.TryGetValue(current, out var container))
                        return container;

                    current = current.parent;
                }
            }

            return transform.gameObject.scene.GetContainer();
        }

        private static void InitLazyBinds()
        {
            if (LazyBinds.Count == 0)
                return;

            //Cache binds
            var binds = new List<GameObjectLazyBind>(LazyBinds);
            LazyBinds.Clear();

            binds.Sort((a, b) => CompareInHierarchy(a.Transform, b.Transform));

            foreach (var bind in binds)
                bind.Callback?.Invoke(bind.Transform.GetOrCreateScope());
        }

        private static int CompareInHierarchy(Transform a, Transform b)
        {
            if (a == b)
                return 0;

            if (a.IsChildOf(b))
                return 1;

            if (b.IsChildOf(a))
                return -1;
            return 0;
        }

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static IContainer GetOrCreateScope(this Transform transform)
        {
            return ContextHelper.GetOrCreateScope(
                transform.GetContainer(),
                transform,
                transform,
                gameObjectContainers);
        }

        public static void LazyGetOrCreateScope(this Transform transform, Action<IContainer> callback)
        {
            if (gameObjectContainers.TryGetValue(transform, out var current))
                callback?.Invoke(current);
            else
                LazyBinds.Add(new GameObjectLazyBind(transform, callback));
        }

        private readonly struct GameObjectLazyBind
        {
            public readonly Transform Transform;
            public readonly Action<IContainer> Callback;

            public GameObjectLazyBind(Transform transform, Action<IContainer> callback)
            {
                Transform = transform;
                Callback = callback;
            }
        }

        private static void BindContainer(this Transform transform, IContainer container)
            => ContextHelper.BindContainer(transform, gameObjectContainers, container);

        public static GameObject InstantiateFromPrefab(
            this IContainer container,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = ContainerInstantiateExt.InstantiateDisabled(prefab, parent, out bool state);
            instance.transform.BindContainer(container);
            ContainerInstantiateExt.SetState(prefab, instance, state);
            return instance;
        }

        public static T InstantiateFromPrefab<T>(
            this IContainer container,
            T prefab,
            Transform parent = null)
            where T : Component
        {
            var instance = ContainerInstantiateExt.InstantiateDisabled(prefab, parent, out bool state);
            instance.transform.BindContainer(container);
            ContainerInstantiateExt.SetState(prefab, instance, state);
            return instance;
        }
    }
}
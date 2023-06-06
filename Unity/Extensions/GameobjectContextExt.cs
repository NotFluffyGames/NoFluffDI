using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public static class GameObjectContextExt
    {
        private static readonly Dictionary<Transform, IReadOnlyContainer> gameObjectContainers = new();
        private static readonly Dictionary<Transform, List<GameObjectLazyBind>> LazyBinds = new();

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

        private static void InitLazyBinds(Transform transform, IContainerBuilder builder)
        {
            if(!LazyBinds.TryGetValue(transform, out var transformBinds))
                return;
            
            if (transformBinds.Count == 0)
                return;

            //Cache binds
            var binds = transformBinds.ToList();
            transformBinds.Clear();
        
            binds.Sort((a, b) => CompareInHierarchy(a.Transform, b.Transform));
        
            foreach (var bind in binds)
                bind.Callback?.Invoke(builder);
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
        public static bool TryGetScope(this Transform transform, out IReadOnlyContainer container)
        {
            return gameObjectContainers.TryGetValue(transform, out container);
        }

        public static IContainerBuilder CreateScope(this Transform transform)
        {
            var builder = ContextHelper.CreateScope(transform.GetContainer(), transform, transform, gameObjectContainers);
            InitLazyBinds(transform, builder);
            return builder;
        }

        public static void LazyBindScope(this Transform transform, Action<IContainerBuilder> bind)
        {
            LazyBinds[transform] ??= new List<GameObjectLazyBind>();
            LazyBinds[transform].Add(new GameObjectLazyBind(transform, bind));
        }

        private readonly struct GameObjectLazyBind
        {
            public readonly Transform Transform;
            public readonly Action<IContainerBuilder> Callback;

            public GameObjectLazyBind(Transform transform, Action<IContainerBuilder> callback)
            {
                Transform = transform;
                Callback = callback;
            }
        }
        
        internal static void BindContainer(this Transform transform, IReadOnlyContainer container)
            => ContextHelper.BindContainer(transform, gameObjectContainers, container);
    }
}
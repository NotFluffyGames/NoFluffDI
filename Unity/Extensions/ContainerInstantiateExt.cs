using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotFluffy.NoFluffDI
{
    public static class ContainerInstantiateExt
    {
        public static GameObject InstantiateFromPrefab(
            this IInstallable installable,
            GameObject prefab,
            Transform parent = null)
            => InstantiateFromPrefab(installable.InstallBindings, prefab, parent);

        public static T InstantiateFromPrefab<T>(
            this IInstallable installable,
            T prefab,
            Transform parent = null)
            where T : Component
            => InstantiateFromPrefab(installable.InstallBindings, prefab, parent);

        public static GameObject InstantiateFromPrefab(
            this Installable installable,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var builder = instance.CreateScope();
            builder.Install(installable);
            builder.Build();
            SetState(prefab, instance, state);
            return instance;
        }

        public static T InstantiateFromPrefab<T>(
            this Installable installable,
            T prefab,
            Transform parent = null)
            where T : Component
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var builder = instance.CreateScope();
            builder.Install(installable);
            builder.Build();
            SetState(prefab, instance, state);
            return instance;
        }

        public static GameObject InstantiateFromPrefab(
            this IEnumerable<IResolverFactory> installable,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var builder = instance.CreateScope();
            builder.Add(installable);
            builder.Build();
            SetState(prefab, instance, state);
            return instance;
        }

        public static T InstantiateFromPrefab<T>(
            this IEnumerable<IResolverFactory> installable,
            T prefab,
            Transform parent = null)
            where T : Component
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var builder = instance.CreateScope();
            builder.Add(installable);
            builder.Build();
            SetState(prefab, instance, state);
            return instance;
        }

        public static GameObject InstantiateFromPrefab(
            this IReadOnlyContainer container,
            Installable installable,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = InstantiateDisabled(prefab, parent, out var originalState);

            instance.transform.BindContainer(container);
            
            if (installable != null)
                instance.transform.LazyBindScope(Bind);
            
            instance.SetActive(originalState);

            return instance;

            void Bind(IContainerBuilder builder)
            {
                builder.Install(installable);
            }
        }

        public static T InstantiateFromPrefab<T>(
            this IReadOnlyContainer container,
            Installable installable,
            T prefab,
            Transform parent = null)
            where T : Component
        {
            var scope = container.Scope(prefab.gameObject.name);

            if (installable != null)
                scope.Install(installable);

            return scope.Build().Container.InstantiateFromPrefab(prefab, parent);
        }

        [UsedImplicitly]
        public static GameObject InstantiateFromPrefab(
            this IReadOnlyContainer container,
            GameObject prefab,
            Transform parent = null)
            => container.InstantiateFromPrefab(null, prefab, parent);

        [UsedImplicitly]
        public static T InstantiateFromPrefab<T>(
            this IReadOnlyContainer container,
            T prefab,
            Transform parent = null)
            where T : Component
            => container.InstantiateFromPrefab(null, prefab, parent);

        private static GameObject InstantiateDisabled(
            GameObject prefab,
            Transform parent,
            out bool originalState)
        {
            originalState = prefab.activeSelf;
            prefab.SetActive(false);
            var instance = Object.Instantiate(prefab, parent);
            prefab.SetActive(originalState);
            return instance;
        }

        private static T InstantiateDisabled<T>(
            T prefab,
            Transform parent,
            out bool originalState)
            where T : Component
        {
            originalState = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            return Object.Instantiate(prefab, parent);
        }

        private static void SetState(GameObject prefab, GameObject instance, bool state)
        {
            prefab.SetActive(state);
            instance.SetActive(state);
        }

        private static void SetState<T>(T prefab, T instance, bool state)
            where T : Component
            => SetState(prefab.gameObject, instance.gameObject, state);
    }
}
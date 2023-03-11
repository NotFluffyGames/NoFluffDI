using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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
            => InstantiateFromPrefab(installable.GetBindings, prefab, parent);

        public static T InstantiateFromPrefab<T>(
            this IInstallable installable,
            T prefab,
            Transform parent = null)
            where T : Component
            => InstantiateFromPrefab(installable.GetBindings, prefab, parent);

        public static GameObject InstantiateFromPrefab(
            this Installable installable,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var container = instance.GetOrCreateScope();
            container.Install(installable);
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
            var container = instance.GetOrCreateScope();
            container.Install(installable);
            SetState(prefab, instance, state);
            return instance;
        }

        public static GameObject InstantiateFromPrefab(
            this IEnumerable<IResolverFactory> installable,
            GameObject prefab,
            Transform parent = null)
        {
            var instance = InstantiateDisabled(prefab, parent, out bool state);
            var container = instance.GetOrCreateScope();
            container.Install(installable);
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
            var container = instance.GetOrCreateScope();
            container.Install(installable);
            SetState(prefab, instance, state);
            return instance;
        }

        public static GameObject InstantiateFromPrefab(
            this IReadOnlyContainer container,
            Installable installable,
            GameObject prefab,
            Transform parent = null)
        {
            var scope = container.Scope(prefab.name);

            if (installable != null)
                scope.Install(installable);

            return scope.InstantiateFromPrefab(prefab, parent);
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

            return scope.InstantiateFromPrefab(prefab, parent);
        }

        public static GameObject InstantiateFromPrefab(
            this IReadOnlyContainer container,
            GameObject prefab,
            Transform parent = null)
            => container.InstantiateFromPrefab(null, prefab, parent);

        public static T InstantiateFromPrefab<T>(
            this IReadOnlyContainer container,
            T prefab,
            Transform parent = null)
            where T : Component
            => container.InstantiateFromPrefab(null, prefab, parent);

        public static GameObject InstantiateDisabled(
            GameObject prefab,
            Transform parent,
            out bool originalState)
        {
            originalState = prefab.activeSelf;
            prefab.SetActive(false);
            return Object.Instantiate(prefab, parent);
        }

        public static IDisposable InstantiateDisabled(
            GameObject prefab,
            Transform parent,
            out GameObject instance)
        {
            var newI = instance = InstantiateDisabled(prefab, parent, out bool originalState);
            return Disposable.Create(Dispose);

            void Dispose()
            {
                prefab.SetActive(originalState);
                newI.SetActive(originalState);
            }
        }

        public static T InstantiateDisabled<T>(
            T prefab,
            Transform parent,
            out bool originalState)
            where T : Component
        {
            originalState = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            return Object.Instantiate(prefab, parent);
        }

        public static IDisposable InstantiateDisabled<T>(
            T prefab,
            Transform parent,
            out T instance)
            where T : Component
        {
            var newI = instance = InstantiateDisabled(prefab, parent, out bool originalState);
            return Disposable.Create(Dispose);

            void Dispose()
            {
                prefab.gameObject.SetActive(originalState);
                newI.gameObject.SetActive(originalState);
            }
        }

        public static void SetState(GameObject prefab, GameObject instance, bool state)
        {
            prefab.SetActive(state);
            instance.SetActive(state);
        }

        public static void SetState<T>(T prefab, T instance, bool state)
            where T : Component
            => SetState(prefab.gameObject, instance.gameObject, state);
    }
}
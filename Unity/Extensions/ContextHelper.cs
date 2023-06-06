using System;
using System.Collections.Generic;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public static class ContextHelper
    {
        public static IContainerBuilder CreateScope<T>(
            IReadOnlyContainer parent,
            object scopeContext,
            T key,
            IDictionary<T, IReadOnlyContainer> containers)
        {
            var builder = parent.Scope(scopeContext);
            builder.RegisterBuildCallback(OnBuild);
            return builder;

            void OnBuild(IReadOnlyContainer container)
            {
                BindContainer(key, containers, container);
            }
        }

        public static void BindContainer<T>(T key, IDictionary<T, IReadOnlyContainer> containers, IReadOnlyContainer container)
        {
            if (containers.TryGetValue(key, out _))
                throw new Exception($"Cannot bind container {container} to {key}, because it already bound to container {container}");

            container.OnDispose.Subscribe(Dispose);
            containers[key] = container;

            void Dispose() => Unbind();

            void Unbind()
            {
                if (containers.TryGetValue(key, out var current)
                    && current == container)
                    containers.Remove(key);
            }
        }
    }
}
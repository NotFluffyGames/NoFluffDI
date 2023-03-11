using System;
using System.Collections.Generic;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public static class ContextHelper
    {
        public static IContainer GetOrCreateScope<T>(
            IReadOnlyContainer parent,
            object scopeContext,
            T key,
            IDictionary<T, IContainer> containers)
        {
            if (containers.TryGetValue(key, out var current))
                return current;

            var scoped = parent.Scope(scopeContext);

            BindContainer(key, containers, scoped);

            return scoped;
        }

        public static void BindContainer<T>(T key, IDictionary<T, IContainer> containers, IContainer container)
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
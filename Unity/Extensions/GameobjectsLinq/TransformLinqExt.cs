using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.GoLinq
{
    public static class TransformLinqExt
    {
        public static IEnumerable<GameObject> GameObjects(this IEnumerable<Transform> transforms)
        {
            return transforms.Select(transform => transform.gameObject);
        }

        public static IEnumerable<Transform> Children(this Transform transform)
        {
            if (transform == null)
                yield break;
            
            for (int i = 0; i < transform.childCount; i++)
                yield return transform.GetChild(i);
        }

        public static IEnumerable<Transform> SelfAndChildren(this Transform transform)
        {
            if (transform == null)
                yield break;
            
            yield return transform;

            foreach (var child in transform.Children())
                yield return child;
        }

        public static IEnumerable<Transform> Parents(this Transform transform)
        {
            if (transform == null)
                yield break;
            
            var parent = transform.parent;

            while (parent != null)
            {
                yield return parent;
                parent = parent.parent;
            }
        }

        public static IEnumerable<Transform> SelfAndParents(this Transform transform)
        {
            if(transform == null)
                yield break;

            yield return transform;

            foreach (var parent in transform.Parents())
                yield return parent;
        }

        public static IEnumerable<Transform> Siblings(this Transform transform)
        {
            if (transform == null)
                return Enumerable.Empty<Transform>();

            var parent = transform.parent;

            if (parent == null)
                return transform.gameObject.scene
                    .GetRootGameObjects()
                    .Transforms()
                    .Except(transform);

            return transform.parent
                .Children()
                .Except(transform);
        }

        public static IEnumerable<Transform> SelfAndSiblings(this Transform transform)
        {
            if (transform == null)
                return Enumerable.Empty<Transform>();

            var parent = transform.parent;

            if (parent == null)
                return transform.gameObject.scene
                    .GetRootGameObjects()
                    .Transforms();

            return transform.parent
                .Children();
        }

        public static IEnumerable<Transform> DeepChildren(this Transform transform)
        {
            if(transform == null)
                return Enumerable.Empty<Transform>();

            return transform
                .Children()
                .SelectMany(child => child.SelfAndDeepChildren());
        }

        public static IEnumerable<Transform> SelfAndDeepChildren(this Transform transform)
        {
            if (transform == null)
                yield break;

            yield return transform;

            foreach (var child in transform.DeepChildren())
                yield return child;
        }
    }
}

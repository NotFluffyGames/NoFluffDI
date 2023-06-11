using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.GoLinq
{
    public static class GameObjectLinqExt
    {
        public static IEnumerable<Transform> Transforms(this IEnumerable<GameObject> gameObjects)
        {
            return gameObjects.Select(go => go.transform);
        }

        public static IEnumerable<T> As<T>(this IEnumerable<GameObject> gameObjects)
        {
            if(gameObjects == null)
                return Enumerable.Empty<T>();

            return gameObjects.Select(go => go.GetComponent<T>());
        }
        
        public static IEnumerable<Component> As(this IEnumerable<GameObject> gameObjects, Type component)
        {
            if(gameObjects == null)
                return Enumerable.Empty<Component>();

            return gameObjects.Select(go => go.GetComponent(component));
        }

        public static IEnumerable<T> AsSelectMany<T>(this IEnumerable<GameObject> gameObjects)
        {
            if(gameObjects == null)
                return Enumerable.Empty<T>();

            return gameObjects.SelectMany(go => go.GetComponents<T>());
        }
        
        public static IEnumerable<Component> AsSelectMany(this IEnumerable<GameObject> gameObjects, Type component)
        {
            if(gameObjects == null)
                return Enumerable.Empty<Component>();

            return gameObjects.SelectMany(go => go.GetComponents(component));
        }

        public static IEnumerable<T> SelectWhereTryGetComponent<T>(this IEnumerable<GameObject> gameObjects)
        {
            if(gameObjects == null)
                return Enumerable.Empty<T>();

            return gameObjects.SelectWhile<GameObject, T>(TryGetComponent);

            bool TryGetComponent(GameObject gameObject, out T component) => gameObject.TryGetComponent(out component);
        }
        
        public static IEnumerable<Component> SelectWhereTryGetComponent(this IEnumerable<GameObject> gameObjects, Type component)
        {
            if(gameObjects == null)
                return Enumerable.Empty<Component>();

            return gameObjects.SelectWhile<GameObject, Component>(TryGetComponent);

            bool TryGetComponent(GameObject gameObject, out Component comp) => gameObject.TryGetComponent(component, out comp);
        }

        public static IEnumerable<GameObject> Children(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.Children().GameObjects();
        }

        public static IEnumerable<GameObject> SelfAndChildren(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.SelfAndChildren().GameObjects();
        }

        public static IEnumerable<GameObject> Parents(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.Parents().GameObjects();
        }

        public static IEnumerable<GameObject> SelfAndParents(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.SelfAndParents().GameObjects();
        }

        public static IEnumerable<GameObject> Siblings(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.Siblings().GameObjects();
        }

        public static IEnumerable<GameObject> SelfAndSiblings(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.SelfAndSiblings().GameObjects();
        }

        public static IEnumerable<GameObject> DeepChildren(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.DeepChildren().GameObjects();
        }

        public static IEnumerable<GameObject> SelfAndDeepChildren(this GameObject gameObject)
        {
            if(gameObject == null)
                return Enumerable.Empty<GameObject>();
            
            return gameObject.transform.SelfAndDeepChildren().GameObjects();
        }
    }
}
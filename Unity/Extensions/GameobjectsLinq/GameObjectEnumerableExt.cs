using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.GoLinq
{
    public static class GameObjectEnumerableExt
    {
        public static void Destroy(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return;
            
            foreach (var gameObject in gameObjects) 
                Object.Destroy(gameObject);
        }

        public static IEnumerable<T> AddComponent<T>(this IEnumerable<GameObject> gameObjects)
            where T : Component
        {
            if(gameObjects == null)
                yield break;

            foreach (var gameObject in gameObjects)
                yield return gameObject.AddComponent<T>();
        }
        
        public static IEnumerable<Component> AddComponent(this IEnumerable<GameObject> gameObjects, Type component)
        {
            if(gameObjects == null)
                yield break;

            foreach (var gameObject in gameObjects)
                yield return gameObject.AddComponent(component);
        }

        public static void SetActive(this IEnumerable<GameObject> gameObjects, bool active)
        {
            if (gameObjects == null)
                return;
            
            foreach (var gameObject in gameObjects) 
                gameObject.SetActive(active);
        }

        public static bool AllActive(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;

            return gameObjects.All(go => go.activeSelf);
        }
        
        public static bool AllInactive(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;

            return gameObjects.All(go => !go.activeSelf);
        }
        
        public static bool AllActiveInHierarchy(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;

            return gameObjects.All(go => go.activeInHierarchy);
        }
        
        public static bool AllInactiveInHierarchy(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;

            return gameObjects.All(go => !go.activeInHierarchy);
        }

        public static bool AllStatic(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;
            
            return gameObjects.All(go => go.isStatic);
        }
        
        public static bool AllNonstatic(this IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null)
                return true;
            
            return gameObjects.All(go => !go.isStatic);
        }

        public static void SetTag(this IEnumerable<GameObject> gameObjects, string tag)
        {
            if (gameObjects == null)
                return;
            
            foreach (var gameObject in gameObjects) 
                gameObject.tag = tag;
        }
        
        public static void SetLayer(this IEnumerable<GameObject> gameObjects, int layer)
        {
            if (gameObjects == null)
                return;
            
            foreach (var gameObject in gameObjects) 
                gameObject.layer = layer;
        }

        public static void SendMessage(this IEnumerable<GameObject> gameObjects, string methodName, object value = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            if (gameObjects == null)
                return;

            foreach (var gameObject in gameObjects) 
                gameObject.SendMessage(methodName, value, options);
        }
    }
}
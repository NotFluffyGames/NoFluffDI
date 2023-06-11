using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.GoLinq
{
    public static class TransformEnumerableExt
    {
        public static void Destroy(this IEnumerable<Transform> transforms)
        {
            if (transforms == null)
                return;
            
            foreach (var transform in transforms) 
                Object.Destroy(transform);
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

        public static IEnumerable<Vector3> Positions(this IEnumerable<Transform> transforms)
        {
            if(transforms == null)
                return Enumerable.Empty<Vector3>();

            return transforms.Select(transform => transform.position);
        }

        public static IEnumerable<Vector3> Forward(this IEnumerable<Transform> transforms)
        {
            if(transforms == null)
                return Enumerable.Empty<Vector3>();

            return transforms.Select(transform => transform.forward);
        }
        
        public static IEnumerable<Vector3> Right(this IEnumerable<Transform> transforms)
        {
            if(transforms == null)
                return Enumerable.Empty<Vector3>();

            return transforms.Select(transform => transform.right);
        }
        
        public static IEnumerable<Vector3> Up(this IEnumerable<Transform> transforms)
        {
            if(transforms == null)
                return Enumerable.Empty<Vector3>();

            return transforms.Select(transform => transform.up);
        }
    }
}
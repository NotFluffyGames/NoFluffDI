using System.IO;
using UnityEditor;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Editor
{
    public static class MenuItems
    {
        public const string MenuItemsRoot = "NotFluffy/NoFluffDI";

        [MenuItem(MenuItemsRoot + "/Create Project Context")]
        public static void CreateProjectContext()
        {
            var instance = ScriptableObject.CreateInstance<ProjectContext>();
            var assetPath = Path.ChangeExtension(Path.Combine("Assets", "Resources", ProjectContext.ProjectContextPath), "asset");
            var assetDirectory = Path.GetDirectoryName(assetPath);

            if (!AssetDatabase.IsValidFolder(assetDirectory))
                ValidatePath(assetDirectory);

            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.Refresh();
        }

        private static void ValidatePath(string assetPath)
        {
            if (!assetPath.StartsWith("Assets"))
                assetPath = Path.Combine("Assets", assetPath);

            var directories = assetPath.Split(Path.DirectorySeparatorChar);

            var currentPath = "";
            for (int i = 0; i < directories.Length; i++)
            {
                var directory = directories[i];
                var newPath = Path.Combine(currentPath, directory);
                if (!AssetDatabase.IsValidFolder(newPath))
                    AssetDatabase.CreateFolder(currentPath, directory);

                currentPath = newPath;
            }
        }
        [MenuItem("GameObject/" + MenuItemsRoot + "/Scene Context")]
        public static void CreateSceneContext()
        {
            _ = new GameObject("Scene Context", typeof(SceneContext));
        }
    }
}
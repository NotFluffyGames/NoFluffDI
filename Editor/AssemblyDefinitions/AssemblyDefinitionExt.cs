using System;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

namespace NotFluffy.Editor
{
    public static class AssemblyDefinitionExt
    {
        public static string GetAssemblyDefinitionPath(this Type type)
            => type.Assembly.GetAssemblyDefinitionPath();

        public static string GetAssemblyDefinitionPath(this Assembly assembly)
            => CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assembly.GetName().Name);
    }
}

using System;
using System.Linq;

namespace NotFluffy
{
    public static class TypeExt
    {
        public static string DisplayName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var genericContract = type.Name.Remove(type.Name.IndexOf('`'));
            var genericArguments = type.GenericTypeArguments.Select(args => args.FullName);
            var commaSeparatedArguments = string.Join(", ", genericArguments);
            return $"{genericContract}<{commaSeparatedArguments}>";
        }
    }
}
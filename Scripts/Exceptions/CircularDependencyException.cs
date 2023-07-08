using System;

namespace NotFluffy.NoFluffDI
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(ResolverID id) : base(GenerateMessage(id))
        {
        }

        private static string GenerateMessage(ResolverID id) => $"Cannot resolve, circular dependency found when trying to resolve '{id.Type.DisplayName()}' with ID '{id.Id}'";
    }
}
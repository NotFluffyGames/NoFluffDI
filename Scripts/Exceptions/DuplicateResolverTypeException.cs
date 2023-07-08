using System;

namespace NotFluffy.NoFluffDI
{
    public class DuplicateResolverTypeException : Exception
    {
        public DuplicateResolverTypeException(ResolverID id) : base(GenerateMessage(id))
        {
        }

        private static string GenerateMessage(ResolverID id) => $"Cannot install resolver, a resolver with type '{id.Type.DisplayName()}' and ID '{id.Id}' already exists.";
    }
}
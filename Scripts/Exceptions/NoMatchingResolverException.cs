using System;

namespace NotFluffy.NoFluffDI
{
    public class NoMatchingResolverException : Exception
    {
        public NoMatchingResolverException(Type contract) : base(GenerateMessage(contract))
        {
        }

        private static string GenerateMessage(Type contract) => $"Cannot resolve contract type '{contract.DisplayName()}'.";
    }
}
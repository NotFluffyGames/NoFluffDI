using System;

namespace NotFluffy.NoFluffDI
{
    public class NoMatchingConverterException : Exception
    {
        public NoMatchingConverterException(Type from, Type to) 
            : base(GenerateMessage(from, to))
        {
        }

        private static string GenerateMessage(Type from, Type to)
            => $"Couldn't find a matching converter from {from.DisplayName()} to {to.DisplayName()}";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public class WrongContractTypeException : Exception
    {
        public WrongContractTypeException(IEnumerable<ResolverID> contract, Type result) : base(GenerateMessage(contract, result))
        {

        }

        private static string GenerateMessage(IEnumerable<ResolverID> contract, Type result) => $"Cannot resolve contract type '{string.Join(", ", contract.Select(id => id.Type.DisplayName()))}', resolve result is of type '{result.DisplayName()}'";
    }
}
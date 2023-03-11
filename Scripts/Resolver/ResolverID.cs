using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ResolverID
    {
        public readonly Type Type;
        public readonly object Id;

        public ResolverID(Type type, object id)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Id = id;
        }

        public override bool Equals(object obj)
            => obj is ResolverID id
               && Type == id.Type
               && Equals(Id, id.Id);

        public override int GetHashCode()
            => HashCode.Combine(Type, Id);
    }
}
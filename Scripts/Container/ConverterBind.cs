using System;

namespace NotFluffy.NoFluffDI
{
    public readonly struct ConverterBind
    {
        public readonly Type From;
        public readonly Converter<object, object> Converter;

        public ConverterBind(Type from, Converter<object, object> converter)
        {
            From = from ?? throw new ArgumentNullException(nameof(from));
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public bool Valid => Converter != null;
    }
}
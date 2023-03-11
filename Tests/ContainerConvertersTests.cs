using System.Collections.Generic;
using NUnit.Framework;

namespace NotFluffy.NoFluffDI.Tests
{
    [TestFixture]
    public class ContainerConvertersTests
    {
        public const string NO_ID_INPUT = "5";
        public const int NO_ID_OUTPUT = 10;
        
        public const string WITH_ID_INPUT = "10";
        public const int WITH_ID_OUTPUT = 20;
        
        public const string WRONG_INPUT = "20";
        
        
        public static int WrongConverter_ConvertAndMultiply(string intStr)
            => int.Parse(intStr) * 3;
        public static int CorrectConverter_ConvertAndMultiply(string intStr)
            => int.Parse(intStr) * 2;
        
        public static void BindWrongInstancesToContainer(IContainer container)
        {
            container.InstallSingle(Resolve.FromInstance(WRONG_INPUT));
        }
        public static void BindCorrectInstancesToContainer(IContainer container)
        {
            container.InstallSingle(Resolve.FromInstance(NO_ID_INPUT));
            container.InstallSingle(Resolve.FromInstance(WITH_ID_INPUT).WithID(ContainerTestsConsts.ID));
        }

        public static void BindWrongConverterToContainer(IContainer container)
        {
            container.SetImplicitConverter<string, int>(WrongConverter_ConvertAndMultiply);
        }
        public static void BindCorrectConverterToContainer(IContainer container)
        {
            container.SetImplicitConverter<string, int>(CorrectConverter_ConvertAndMultiply);
        }
        
        public static IEnumerable<IReadOnlyContainer> Containers
        {
            get
            {
                IContainer parent;
                IContainer child;
                
                var direct = new Container("direct");
                BindCorrectConverterToContainer(direct);
                BindCorrectInstancesToContainer(direct);

                yield return direct;

                parent = new Container("parent1");
                BindCorrectConverterToContainer(parent);

                child = parent.Scope("child1");
                BindCorrectInstancesToContainer(child);
                        
                //Parent has converter, child has the instances to convert
                yield return child;

                parent = new Container("parent2");
                BindCorrectConverterToContainer(parent);
                BindWrongInstancesToContainer(parent);

                child = parent.Scope("child2");
                BindCorrectInstancesToContainer(child);
                        
                //Parent has converter, child has the instances to convert
                yield return child;

                parent = new Container("parent3");
                BindCorrectInstancesToContainer(parent);
                    
                child = parent.Scope("child3");
                BindCorrectConverterToContainer(child);
                        
                //Parent has instances to convert, child has the converter
                yield return child;

                parent = new Container("parent4");
                BindCorrectInstancesToContainer(parent);
                BindWrongConverterToContainer(parent);
                    
                child = parent.Scope("child4");
                BindCorrectConverterToContainer(child);
                        
                //Parent has instances to convert, child has the converter
                yield return child;
            }
        }

        [TestCaseSource(nameof(Containers))]
        public void CanConvert_StringToInt_True(IReadOnlyContainer container)
        {
            Assert.True(container.CanConvert<string, int>());
        }
        
        [TestCaseSource(nameof(Containers))]
        public void CanConvert_StringToFloat_False(IReadOnlyContainer container)
        {
            Assert.False(container.CanConvert<string, float>());
        }

        [TestCaseSource(nameof(Containers))]
        public void Convert_StringToInt_ConvertedString(IReadOnlyContainer container)
        {
            Assert.AreEqual(container.Convert<string, int>(NO_ID_INPUT), NO_ID_OUTPUT);
        }

        [TestCaseSource(nameof(Containers))]
        public void Convert_StringToFloat_NoMatchingConverterException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingConverterException>(() => container.Convert<string, float>(NO_ID_INPUT));
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Contains_IntFromConverter_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<int>());
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Contains_FloatFromConverter_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<float>());
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Contains_IntFromConverterWithID_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<int>(ContainerTestsConsts.ID));
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Contains_FloatFromConverterWithID_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<float>(ContainerTestsConsts.ID));
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Contains_IntFromConverterWithWrongID_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<int>(ContainerTestsConsts.WRONG_ID));
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Resolve_Int_ConvertedString(IReadOnlyContainer container)
        {
            Assert.AreEqual(container.Resolve<int>(), NO_ID_OUTPUT);
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Resolve_IntWithId_ConvertedString(IReadOnlyContainer container)
        {
            Assert.AreEqual(container.Resolve<int>(ContainerTestsConsts.ID), WITH_ID_OUTPUT);
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Resolve_Float_NoMatchingResolverException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingResolverException>(() => container.Resolve<float>());
        }
        
        [TestCaseSource(nameof(Containers))]
        public void Resolve_FloatWithId_NoMatchingResolverException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingResolverException>(() => container.Resolve<float>(ContainerTestsConsts.ID));
        }
    }
}
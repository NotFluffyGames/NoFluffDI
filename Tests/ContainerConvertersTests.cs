using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

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

        public static void BindWrongInstancesToContainer(IContainerBuilder container)
        {
            container.Add(Resolve.FromInstance(WRONG_INPUT));
        }

        public static void BindCorrectInstancesToContainer(IContainerBuilder container)
        {
            container.Add(Resolve.FromInstance(NO_ID_INPUT));
            container.Add(Resolve.FromInstance(WITH_ID_INPUT).WithID(ContainerTestsConsts.ID));
        }

        public static void BindWrongConverterToContainer(IContainerBuilder container)
        {
            container.SetImplicitConverter<string, int>(WrongConverter_ConvertAndMultiply);
        }

        public static void BindCorrectConverterToContainer(IContainerBuilder container)
        {
            container.SetImplicitConverter<string, int>(CorrectConverter_ConvertAndMultiply);
        }

        public static IEnumerable<IReadOnlyContainer> Containers
        {
            get
            {
                IReadOnlyContainer parent;
                IReadOnlyContainer child;

                var direct = new ContainerBuilder("direct");
                BindCorrectConverterToContainer(direct);
                BindCorrectInstancesToContainer(direct);

                yield return direct.Build();

                IContainerBuilder builder = new ContainerBuilder("parent1");
                BindCorrectConverterToContainer(builder);
                parent = builder.Build();


                builder = parent.Scope("child1");
                BindCorrectInstancesToContainer(builder);
                child = builder.Build();

                //Parent has converter, child has the instances to convert
                yield return child;

                builder = new ContainerBuilder("parent2");
                BindCorrectConverterToContainer(builder);
                BindWrongInstancesToContainer(builder);
                parent = builder.Build();

                builder = parent.Scope("child2");
                BindCorrectInstancesToContainer(builder);
                child = builder.Build();

                //Parent has converter, child has the instances to convert
                yield return child;

                builder = new ContainerBuilder("parent3");
                BindCorrectInstancesToContainer(builder);
                parent = builder.Build();

                builder = parent.Scope("child3");
                BindCorrectConverterToContainer(builder);
                child = builder.Build();

                //Parent has instances to convert, child has the converter
                yield return child;

                builder = new ContainerBuilder("parent4");
                BindCorrectInstancesToContainer(builder);
                BindWrongConverterToContainer(builder);
                parent = builder.Build();

                builder = parent.Scope("child4");
                BindCorrectConverterToContainer(builder);
                child = builder.Build();

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

        [UnityTest]
        public IEnumerator Resolve_Int_ConvertedString()
        {
            foreach (var container in Containers)
                yield return container
                    .Resolve<int>()
                    .ToCoroutine(result => Assert.AreEqual(result, NO_ID_OUTPUT));
        }

        [UnityTest]
        public IEnumerator Resolve_IntWithId_ConvertedString()
        {
            foreach (var container in Containers)
                yield return container
                    .Resolve<int>(ContainerTestsConsts.ID)
                    .ToCoroutine(result => Assert.AreEqual(result, WITH_ID_OUTPUT));
        }

        [UnityTest]
        public IEnumerator Resolve_Float_NoMatchingResolverException()
        {
            foreach (var container in Containers)
                yield return container
                    .Resolve<float>()
                    .AssertThrows<float, NoMatchingResolverException>()
                    .ToCoroutine();
        }

        [UnityTest]
        public IEnumerator Resolve_FloatWithId_NoMatchingResolverException()
        {
            foreach (var container in Containers)
                yield return container
                    .Resolve<float>(ContainerTestsConsts.ID)
                    .AssertThrows<float, NoMatchingResolverException>()
                    .ToCoroutine();
        }
    }
}
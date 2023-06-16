using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NotFluffy.NoFluffDI.Tests
{
    public static class UniTaskAssertExt
    {
        public static async UniTask AssertThrows<T, TException>(this UniTask<T> task)
        {
            Exception exception = null;
            try
            {
                await task;
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.IsInstanceOf<TException>(exception);
        }

        public static async UniTask AssertThrows<TException>(this UniTask task)
            where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException e)
            {
                Assert.IsInstanceOf<TException>(e);
                return;
            }

            Assert.Fail();
        }
    }


    [TestFixture]
    public class ContainerTests
    {
        private class TypeWithState
        {
            public object State { get; set; }
        }

        public static IEnumerable<IReadOnlyContainer> ContainersForResolve
            => GetContainersForResolve();

        public static IEnumerable<IReadOnlyContainer> ContainersForResolveWithID
            => GetContainersForResolve(ContainerTestsConsts.ID);

        private static IEnumerable<IReadOnlyContainer> GetContainersForResolve(object id = null)
        {
            //FromInstance
            yield return Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .BuildContainer("direct")
                .Container;

            var parent = Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .BuildContainer("parentWithCorrectInstances")
                .Container;

            yield return parent
                .Scope("childOfParentWithCorrectInstances")
                .Build()
                .Container;

            parent = new ContainerBuilder("parent").Build().Container;

            yield return Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .BuildContainer("childWithInstances", parent)
                .Container;

            parent = Resolve
                .FromInstance(ContainerTestsConsts.WRONG_INPUT)
                .WithID(id)
                .BuildContainer("parentWithWrongInstances")
                .Container;

            yield return Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .BuildContainer("childOfParentWithWrongInstances", parent)
                .Container;

            //FromMethod
            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .BuildContainer("direct")
                .Container;

            parent = Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .BuildContainer("parentWithCorrectInstances")
                .Container;

            yield return parent.Scope("childOfParentWithCorrectInstances").Build().Container;

            parent = new ContainerBuilder("parent").Build().Container;
            
            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .BuildContainer("childWithInstances", parent)
                .Container;

            parent = Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.WRONG_INPUT))
                .WithID(id)
                .BuildContainer("parentWithWrongInstances")
                .Container;

            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .BuildContainer("childOfParentWithWrongInstances", parent)
                .Container;
        }


        [TestCaseSource(nameof(ContainersForResolve))]
        public void Contains_StringFromInstalledContainer_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<string>());
        }

        [TestCaseSource(nameof(ContainersForResolve))]
        public void Contains_IntFromInstalledContainer_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<int>());
        }

        [UnityTest]
        public IEnumerator Resolve_StringFromInstalledContainer_True()
        {
            foreach (var container in ContainersForResolve)
                yield return container
                    .Resolve<string>()
                    .ToCoroutine(r => Assert.AreEqual(r, ContainerTestsConsts.CORRECT_INPUT));
        }

        [UnityTest]
        public IEnumerator Resolve_IntFromNotInstalledContainer_NoMatchingResolverException()
        {
            foreach (var container in ContainersForResolve)
                yield return container
                    .Resolve<int>()
                    .AssertThrows<int, NoMatchingResolverException>()
                    .ToCoroutine();
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_StringFromInstalledContainerWithId_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<string>(ContainerTestsConsts.ID));
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_StringFromInstalledContainerWithWrongId_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<string>(ContainerTestsConsts.WRONG_ID));
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_IntFromInstalledContainerWithId_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<int>(ContainerTestsConsts.ID));
        }

        [UnityTest]
        public IEnumerator Resolve_StringFromInstalledContainerWithId_True()
        {
            foreach (var container in ContainersForResolveWithID)
                yield return container
                    .Resolve<string>(ContainerTestsConsts.ID)
                    .ToCoroutine(r => Assert.AreEqual(r, ContainerTestsConsts.CORRECT_INPUT));
        }

        [UnityTest]
        public IEnumerator Resolve_StringFromInstalledContainerWithWrongId_NoMatchingResolverException()
        {
            foreach (var container in ContainersForResolveWithID)
                yield return container
                    .Resolve<string>(ContainerTestsConsts.WRONG_ID)
                    .AssertThrows<string, NoMatchingResolverException>()
                    .ToCoroutine();
        }

        [UnityTest]
        public IEnumerator Resolve_IntFromNotInstalledContainerWithId_NoMatchingResolverException()
        {
            foreach (var container in ContainersForResolveWithID)
            {
                yield return container
                    .Resolve<int>()
                    .AssertThrows<int, NoMatchingResolverException>()
                    .ToCoroutine();
            }
        }

        [UnityTest]
        public IEnumerator Resolve_AsTransient_ShouldAlwaysReturnANewInstance()
        {
            var container = Resolve
                .FromNew<TypeWithState>()
                .AsTransient()
                .BuildContainer("container")
                .Container;


            TypeWithState result1 = default;
            TypeWithState result2 = default;

            yield return container.Resolve<TypeWithState>().ToCoroutine(r => result1 = r);
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => result2 = r);

            result1.State = ContainerTestsConsts.WRONG_INPUT;
            Assert.AreNotEqual(result2.State, result1.State);
        }

        [UnityTest]
        public IEnumerator Resolve_AsSingle_ShouldAlwaysReturnSameInstance()
        {
            var container = Resolve
                .FromNew<TypeWithState>()
                .AsSingle()
                .BuildContainer("container")
                .Container;

            TypeWithState result = null;
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => result = r);
            result.State = ContainerTestsConsts.CORRECT_INPUT;
            Assert.AreEqual(result.State, ContainerTestsConsts.CORRECT_INPUT);
        }

        [UnityTest]
        public IEnumerator Resolve_FromMethod_ShouldExecuteMethod()
        {
            var container = Resolve
                .FromMethodAsync(Create)
                .BuildContainer("container")
                .Container;

            TypeWithState result = default;

            yield return container
                .Resolve<TypeWithState>()
                .ToCoroutine(r => result = r);

            Assert.AreEqual(result.State, ContainerTestsConsts.CORRECT_INPUT);

            UniTask<TypeWithState> Create() => new(new TypeWithState { State = ContainerTestsConsts.CORRECT_INPUT });
        }

        [UnityTest]
        public IEnumerator Resolve_ResolveFromInsideResolve_CorrectlyResolve()
        {
            using var builder = new ContainerBuilder("container");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            builder.Add(Resolve.FromMethodAsync(async resolver => new TypeWithState { State = await resolver.Resolve<string>() }).AsTransient());

            var container = builder.Build().Container;
            TypeWithState output = default;
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => output = r);

            Assert.AreEqual(output.State, ContainerTestsConsts.CORRECT_INPUT);
        }

        //Lazy/None lazy
        [Test]
        public void Resolve_InstallAsLazyAndDoesntResolve_ShouldNotResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethodAsync(GetValue).Lazy());

            builder.Build();

            Assert.False(wasResolved);

            UniTask<int> GetValue()
            {
                wasResolved = true;
                return new UniTask<int>(5);
            }
        }

        [UnityTest]
        public IEnumerator Resolve_InstallAsLazyAndResolve_ShouldResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethodAsync(GetValue).Lazy());

            var container = builder.Build().Container;
            yield return container.Resolve<int>().ToCoroutine();

            Assert.True(wasResolved);

            UniTask<int> GetValue()
            {
                wasResolved = true;
                return new UniTask<int>(5);
            }
        }

        [Test]
        public void Resolve_InstallAsNonLazyAndDoesntResolve_ShouldResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethodAsync(GetValue).NonLazy());

            builder.Build();

            Assert.True(wasResolved);

            UniTask<int> GetValue()
            {
                wasResolved = true;
                return new UniTask<int>(5);
            }
        }
    }
}
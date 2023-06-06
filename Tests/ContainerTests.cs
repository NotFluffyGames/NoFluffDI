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
                .CreateContainer("direct");

            var parent = Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .CreateContainer("parentWithCorrectInstances");

            yield return parent
                .Scope("childOfParentWithCorrectInstances")
                .Build();

            parent = new ContainerBuilder("parent").Build();

            yield return Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .CreateContainer("childWithInstances", parent);

            parent = Resolve
                .FromInstance(ContainerTestsConsts.WRONG_INPUT)
                .WithID(id)
                .CreateContainer("parentWithWrongInstances");

            yield return Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .WithID(id)
                .CreateContainer("childOfParentWithWrongInstances", parent);

            //FromMethod
            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .CreateContainer("direct");

            parent = Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .CreateContainer("parentWithCorrectInstances");

            yield return parent.Scope("childOfParentWithCorrectInstances").Build();

            parent = new ContainerBuilder("parent").Build();
            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .CreateContainer("childWithInstances", parent);

            parent = Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.WRONG_INPUT))
                .WithID(id)
                .CreateContainer("parentWithWrongInstances");

            yield return Resolve
                .FromMethod<string>(() => new(ContainerTestsConsts.CORRECT_INPUT))
                .WithID(id)
                .CreateContainer("childOfParentWithWrongInstances", parent);
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
                .CreateContainer("container");

            
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
                .CreateContainer("container");

            TypeWithState result = null;
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => result = r);
            result.State = ContainerTestsConsts.CORRECT_INPUT;
            Assert.AreEqual(result.State, ContainerTestsConsts.CORRECT_INPUT);
        }

        [UnityTest]
        public IEnumerator Resolve_FromMethod_ShouldExecuteMethod()
        {
            var container = Resolve
                .FromMethod<TypeWithState>(() => new(new TypeWithState { State = ContainerTestsConsts.CORRECT_INPUT }))
                .CreateContainer("container");

            TypeWithState result = default;
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => result = r);
            Assert.AreEqual(result.State, ContainerTestsConsts.CORRECT_INPUT);
        }

        [UnityTest]
        public IEnumerator Resolve_ResolveFromInsideResolve_CorrectlyResolve()
        {
            var builder = new ContainerBuilder("container");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            builder.Add(Resolve.FromMethod(async resolver => new TypeWithState { State = await resolver.Resolve<string>() }).AsTransient());
            
            var container = builder.Build();
            TypeWithState output = default;
            yield return container.Resolve<TypeWithState>().ToCoroutine(r => output = r);

            Assert.AreEqual(output.State, ContainerTestsConsts.CORRECT_INPUT);
        }

        //Lazy/None lazy
        [Test]
        public void Resolve_InstallAsLazyAndDoesntResolve_ShouldNotResolve()
        {
            var wasResolved = false;
            var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue).Lazy());

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
            var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue).Lazy());

            var container = builder.Build();
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
            var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue).NonLazy());

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
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NotFluffy.NoFluffDI.Tests
{
    public interface ITestInput
        {
            string Value { get; }
        }
        public class CorrectInput : ITestInput
        {
            public string Value => ContainerTestsConsts.CORRECT_INPUT;
        }
        
        public class WrongInput : ITestInput
        {
            public string Value => ContainerTestsConsts.WRONG_INPUT;
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
            #region FromInstance

            var correct = new CorrectInput();
            var wrong = new WrongInput();
            
            //Directly
            yield return Resolve
                .FromInstance<ITestInput>(correct)
                .WithID(id)
                .BuildContainer("direct")
                .Container;

            //Resolve through child without binds
            var parent = Resolve
                .FromInstance<ITestInput>(correct)
                .WithID(id)
                .BuildContainer("parentWithCorrectInput")
                .Container;

            yield return parent
                .Scope("childOfParentWithCorrectInput")
                .Build()
                .Container;

            //Resolve through child overrding parent's binds
            parent = Resolve
                .FromInstance<ITestInput>(wrong)
                .WithID(id)
                .BuildContainer("parentWithWrongInput")
                .Container;

            yield return Resolve
                .FromInstance<ITestInput>(correct)
                .WithID(id)
                .BuildContainer("childOfParentWithWrongInput", parent)
                .Container;

            #endregion

            #region FromMethod

            //Directly
            yield return Resolve
                .FromMethod<ITestInput>(() => new CorrectInput())
                .WithID(id)
                .BuildContainer("direct")
                .Container;

            //Resolve through child without binds
            parent = Resolve
                .FromMethod<ITestInput>(() => new CorrectInput())
                .WithID(id)
                .BuildContainer("parentWithCorrectInput")
                .Container;

            yield return parent
                .Scope("childOfParentWithCorrectInput")
                .Build()
                .Container;

            //Resolve through child overrding parent's binds
            parent = Resolve
                .FromMethod<ITestInput>(() => new WrongInput())
                .WithID(id)
                .BuildContainer("parentWithWrongInstances")
                .Container;

            yield return Resolve
                .FromMethod<ITestInput>(() => new CorrectInput())
                .WithID(id)
                .BuildContainer("childOfParentWithWrongInstances", parent)
                .Container;

            #endregion

            #region FromNew

            //Directly
            yield return Resolve
                .FromNew<ITestInput, CorrectInput>()
                .WithID(id)
                .BuildContainer("direct")
                .Container;

            //Resolve through child without binds
            parent = Resolve
                .FromNew<ITestInput, CorrectInput>()
                .WithID(id)
                .BuildContainer("parentWithCorrectInput")
                .Container;

            yield return parent
                .Scope("childOfParentWithCorrectInput")
                .Build()
                .Container;

            //Resolve through child overrding parent's binds
            parent = Resolve
                .FromNew<ITestInput, WrongInput>()
                .WithID(id)
                .BuildContainer("parentWithWrongInstances")
                .Container;

            yield return Resolve
                .FromNew<ITestInput, CorrectInput>()
                .WithID(id)
                .BuildContainer("childOfParentWithWrongInstances", parent)
                .Container;

            #endregion
        }


        [TestCaseSource(nameof(ContainersForResolve))]
        public void Contains_FromInstalledContainer_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<ITestInput>());
        }

        [TestCaseSource(nameof(ContainersForResolve))]
        public void Contains_IntFromInstalledContainer_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<int>());
        }

        [TestCaseSource(nameof(ContainersForResolve))]
        public void Resolve_FromInstalledContainer_CorrectInput(IReadOnlyContainer container)
        {
            Assert.IsInstanceOf<CorrectInput>(container.Resolve<ITestInput>());
        }
        
        [TestCaseSource(nameof(ContainersForResolve))]
        public void Resolve_IntFromNotInstalledContainer_NoMatchingResolverException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingResolverException>(() => container.Resolve<int>());
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_StringFromInstalledContainerWithId_True(IReadOnlyContainer container)
        {
            Assert.True(container.Contains<ITestInput>(ContainerTestsConsts.ID));
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_StringFromInstalledContainerWithWrongId_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<ITestInput>(ContainerTestsConsts.WRONG_ID));
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Contains_IntFromInstalledContainerWithId_False(IReadOnlyContainer container)
        {
            Assert.False(container.Contains<int>(ContainerTestsConsts.ID));
        }

        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Resolve_FromInstalledContainerWithId_CorrectInput(IReadOnlyContainer container)
        {
            Assert.IsInstanceOf<CorrectInput>(container.Resolve<ITestInput>(ContainerTestsConsts.ID));
        }
        
        [TestCaseSource(nameof(ContainersForResolveWithID))]
        public void Resolve_FromInstalledContainerWithWrongId_NoMatchingResolverException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingResolverException>(() => container.Resolve<ITestInput>(ContainerTestsConsts.WRONG_ID));
        }
        
        [TestCaseSource(nameof(ContainersForResolve))]
        public void Resolve_IntFromNotInstalledContainerWithId_NoMatchingResolverException(IReadOnlyContainer container)
        {
            Assert.Throws<NoMatchingResolverException>(() => container.Resolve<int>());
        }

        [Test]
        public void Resolve_AsTransient_ShouldAlwaysReturnANewInstance()
        {
            var container = Resolve
                .FromNew<TypeWithState>()
                .AsTransient()
                .BuildContainer("container")
                .Container;


            var result1 = container.Resolve<TypeWithState>();
            var result2 = container.Resolve<TypeWithState>();

            //Result2 doesn't change because it's a different instance
            result1.State = ContainerTestsConsts.WRONG_INPUT;
            Assert.AreNotEqual(result2.State, result1.State);
        }

        [Test]
        public void Resolve_AsSingle_ShouldAlwaysReturnSameInstance()
        {
            var container = Resolve
                .FromNew<TypeWithState>()
                .AsSingle()
                .BuildContainer("container")
                .Container;

            var result1 = container.Resolve<TypeWithState>();
            var result2 = container.Resolve<TypeWithState>();
            
            //Result2 changes because it's the same instance
            result1.State = ContainerTestsConsts.WRONG_INPUT;
            Assert.AreEqual(result1.State, result2.State);
        }

        [Test]
        public void Resolve_FromMethod_ShouldExecuteMethod()
        {
            var methodCalled = false;
            
            var container = Resolve
                .FromMethod(Create)
                .BuildContainer("container")
                .Container;

            container.Resolve<TypeWithState>();

            Assert.IsTrue(methodCalled);

            TypeWithState Create()
            {
                methodCalled = true;
                return new();
            }
        }

        [Test]
        public void Resolve_ResolveFromInsideResolve_CorrectlyResolve()
        {
            using var builder = new ContainerBuilder("container");
            
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            builder.Add(Resolve.FromMethod(Create).AsTransient());

            var container = builder.Build().Container;
            var output = container.Resolve<TypeWithState>();

            Assert.AreEqual(output.State, ContainerTestsConsts.CORRECT_INPUT);
            
            TypeWithState Create(IResolutionContext resolver) => new() { State = resolver.Resolve<string>() };
        }

        //Lazy/None lazy
        [Test]
        public void Resolve_InstallAsLazyAndDoesntResolve_ShouldNotResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue));

            builder.Build();

            Assert.False(wasResolved);

            UniTask<int> GetValue()
            {
                wasResolved = true;
                return new UniTask<int>(5);
            }
        }

        [Test]
        public void Resolve_InstallAsLazyAndResolve_ShouldResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue));

            Assert.False(wasResolved);
            
            var container = builder.Build().Container;
            container.Resolve<int>();

            Assert.True(wasResolved);

            int GetValue()
            {
                wasResolved = true;
                return 5;
            }
        }

        [Test]
        public void Resolve_InstallAsNonLazyAndDoesntResolve_ShouldResolve()
        {
            var wasResolved = false;
            using var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue));
            builder.MarkNoneLazy<int>();

            builder.Build();

            Assert.True(wasResolved);

            UniTask<int> GetValue()
            {
                wasResolved = true;
                return new UniTask<int>(5);
            }
        }

        [Test]
        public void Resolve_RecursivelyResolveAType_ThrowCircularDependencyException()
        {
            var container = Resolve
                .FromMethod(context => context.Resolve<ITestInput>())
                .BuildContainer("Recursively resolved container")
                .Container;
            
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITestInput>());
        }
        
        [Test]
        public void ResolveAsync_TypesDependOnEachOther_ThrowCircularDependencyException()
        {
            var container = Resolvers()
                .BuildContainer("Recursively resolved container")
                .Container;
            
            Assert.Throws<CircularDependencyException>(() => container.Resolve<string>());

            IEnumerable<IResolverFactory> Resolvers()
            {
                int IntResolver(IResolutionContext context)
                {
                    return int.Parse(context.Resolve<string>());
                }

                string StringResolver(IResolutionContext context)
                {
                    return context.Resolve<int>().ToString();
                }

                yield return Resolve.FromMethod(IntResolver);
                yield return Resolve.FromMethod(StringResolver);
            }
        }
    }
}
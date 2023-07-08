using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NotFluffy.NoFluffDI.Tests
{
    [TestFixture]
    public class ContainerAsyncTests
    {
        public IEnumerable<IReadOnlyContainer> ContainersForAsyncResolve
        {
            get
            {
                //Directly
                yield return Resolve
                    .FromMethodAsync<ITestInput>(() => new(new CorrectInput()))
                    .BuildContainer("direct")
                    .Container;

                //Resolve through child without binds
                var parent = Resolve
                    .FromMethodAsync<ITestInput>(() => new(new CorrectInput()))
                    .BuildContainer("parentWithCorrectInput")
                    .Container;

                yield return parent
                    .Scope("childOfParentWithCorrectInput")
                    .Build()
                    .Container;

                //Resolve through child overrding parent's binds
                parent = Resolve
                    .FromMethodAsync<ITestInput>(() => new(new WrongInput()))
                    .BuildContainer("parentWithWrongInstances")
                    .Container;

                yield return Resolve
                    .FromMethodAsync<ITestInput>(() => new(new CorrectInput()))
                    .BuildContainer("childOfParentWithWrongInstances", parent)
                    .Container;
            }
        }
        
        [UnityTest]
        public IEnumerator ResolveAsync_RecursivelyResolveAType_ThrowCircularDependencyException()
        {
            var container = Resolve
                .FromMethodAsync(context => context.ResolveAsync<ITestInput>())
                .BuildContainer("Recursively resolved container")
                .Container;

            Exception e = default;
            yield return container.ResolveAsync<ITestInput>().ToCoroutine(_ => { }, exception => e = exception);
            
            Assert.IsInstanceOf<CircularDependencyException>(e);
        }
        
        [UnityTest]
        public IEnumerator ResolveAsync_TypesDependOnEachOther_ThrowCircularDependencyException()
        {
            var container = Resolvers()
                .BuildContainer("Recursively resolved container")
                .Container;

            Exception e = default;
            yield return container.ResolveAsync<string>().ToCoroutine(_ => { }, exception => e = exception);
            
            Assert.IsInstanceOf<CircularDependencyException>(e);

            IEnumerable<IResolverFactory> Resolvers()
            {
                async UniTask<int> IntResolver(IResolutionContext context)
                {
                    await context.ResolveAsync<string>();
                    return 5;
                }

                async UniTask<string> StringResolver(IResolutionContext context)
                {
                    await context.ResolveAsync<int>();
                    return "5";
                }

                yield return Resolve.FromMethodAsync(IntResolver);
                yield return Resolve.FromMethodAsync(StringResolver);
            }
        }
        
        // [UnityTest]
        // public IEnumerator ResolveAsync_StringFromInstalledContainer_True()
        // {
        //     foreach (var container in ContainersForResolve)
        //         yield return container
        //             .ResolveAsync<string>()
        //             .ToCoroutine(r => Assert.AreEqual(r, ContainerTestsConsts.CORRECT_INPUT));
        // }
    }
}
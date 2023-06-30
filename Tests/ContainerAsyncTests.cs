using System.Collections.Generic;
using NUnit.Framework;

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
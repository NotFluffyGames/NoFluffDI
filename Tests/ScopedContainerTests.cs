using NUnit.Framework;
using System.Collections.Generic;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI.Tests
{
    public class ScopedContainerTests
    {
        [Test]
        public void Resolve_OverrideParentInstallInChildAndResolveChild_ResolveChildValue()
        {
            using var parent = new Container("parent");
            parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT));

            using var child = parent.Scope("child");
            child.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            Assert.AreEqual(child.Resolve<string>(), ContainerTestsConsts.CORRECT_INPUT);
        }

        [Test]
        public void Resolve_OverrideParentInstallInChildAndResolveParent_ResolveParentValue()
        {
            using var parent = new Container("parent");
            parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));

            using var child = parent.Scope("child");
            child.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT));
            
            Assert.AreEqual(parent.Resolve<string>(), ContainerTestsConsts.CORRECT_INPUT);
        }

        [Test]
        public void Resolve_ResolveFromChildWhoWasCreatedBeforeBindWasInstalledInParent_ResolveParentValue()
        {
            using var parent = new Container("parent");

            using var child = parent.Scope("child");
            
            //Install a new binding AFTER the child container was created
            parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));

            Assert.AreEqual(child.Resolve<string>(), ContainerTestsConsts.CORRECT_INPUT);
        }

        [Test]
        public void Dispose_DisposingParentContainerWithChildren_DisposeInDFS()
        {
            var disposedOrder = new List<string>();
            string[] shouldBe;
            using (var parent = new Container("Parent"))
            {
                parent.OnDispose.Subscribe(() => disposedOrder.Add(parent.ToString())); 
                
                var firstMiddleChild = parent.Scope("firstMiddleChild");
                firstMiddleChild.OnDispose.Subscribe(() => disposedOrder.Add(firstMiddleChild.ToString())); 

                var firstYoungestChild = firstMiddleChild.Scope("firstYoungestChild");
                firstYoungestChild.OnDispose.Subscribe(() => disposedOrder.Add(firstYoungestChild.ToString())); 

                var secondMiddleChild = parent.Scope("secondMiddleChild");
                secondMiddleChild.OnDispose.Subscribe(() => disposedOrder.Add(secondMiddleChild.ToString())); 

                var secondYoungestChild = secondMiddleChild.Scope("secondYoungestChild");
                secondYoungestChild.OnDispose.Subscribe(() => disposedOrder.Add(secondYoungestChild.ToString())); 

                shouldBe = new[] { secondYoungestChild.ToString(), secondMiddleChild.ToString(), firstYoungestChild.ToString(), firstMiddleChild.ToString(), parent.Context.ToString() };
            }
            
            CollectionAssert.AreEqual(disposedOrder, shouldBe);
        }
    }
}
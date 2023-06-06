using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NotFluffy.NoFluffRx;
using UnityEngine.TestTools;

namespace NotFluffy.NoFluffDI.Tests
{
    public class ScopedContainerTests
    {
        [UnityTest]
        public IEnumerator Resolve_OverrideParentInstallInChildAndResolveChild_ResolveChildValue()
        {
            IContainerBuilder builder = new ContainerBuilder("parent");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT));
            
            var parent = builder.Build();

            builder = parent.Scope("child");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            
            var child = builder.Build();
            string result = default;
            yield return child
                .Resolve<string>()
                .ToCoroutine(r => result = r);

            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }

        [UnityTest]
        public IEnumerator Resolve_OverrideParentInstallInChildAndResolveParent_ResolveParentValue()
        {
            IContainerBuilder builder = new ContainerBuilder();

            var parent = Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).CreateContainer("parent");

            var unused = Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT).CreateContainer("child", parent);

            string result = null;
            yield return parent.Resolve<string>().ToCoroutine(r => result = r);
            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }

        // [Test]
        // public void Resolve_ResolveFromChildWhoWasCreatedBeforeBindWasInstalledInParent_ResolveParentValue()
        // {
        //     var parent = new Container("parent");
        //
        //     using var child = parent.Scope("child");
        //     
        //     //Install a new binding AFTER the child container was created
        //     parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
        //
        //     Assert.AreEqual(child.Resolve<string>(), ContainerTestsConsts.CORRECT_INPUT);
        // }

        // [Test]
        // public void Dispose_DisposingParentContainerWithChildren_DisposeInDFS()
        // {
        //     var disposedOrder = new List<string>();
        //     string[] shouldBe;
        //     using (var parent = new Container("Parent"))
        //     {
        //         parent.OnDispose.Subscribe(() => disposedOrder.Add(parent.ToString())); 
        //         
        //         var firstMiddleChild = parent.Scope("firstMiddleChild");
        //         firstMiddleChild.OnDispose.Subscribe(() => disposedOrder.Add(firstMiddleChild.ToString())); 
        //
        //         var firstYoungestChild = firstMiddleChild.Scope("firstYoungestChild");
        //         firstYoungestChild.OnDispose.Subscribe(() => disposedOrder.Add(firstYoungestChild.ToString())); 
        //
        //         var secondMiddleChild = parent.Scope("secondMiddleChild");
        //         secondMiddleChild.OnDispose.Subscribe(() => disposedOrder.Add(secondMiddleChild.ToString())); 
        //
        //         var secondYoungestChild = secondMiddleChild.Scope("secondYoungestChild");
        //         secondYoungestChild.OnDispose.Subscribe(() => disposedOrder.Add(secondYoungestChild.ToString())); 
        //
        //         shouldBe = new[] { secondYoungestChild.ToString(), secondMiddleChild.ToString(), firstYoungestChild.ToString(), firstMiddleChild.ToString(), parent.Context.ToString() };
        //     }
        //     
        //     CollectionAssert.AreEqual(disposedOrder, shouldBe);
        // }
    }
}
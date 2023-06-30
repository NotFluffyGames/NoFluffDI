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
        [Test]
        public void Resolve_OverrideParentInstallInChildAndResolveChild_ResolveChildValue()
        {
            IContainerBuilder builder = new ContainerBuilder("parent");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT));
            
            var parent = builder.Build().Container;

            builder = parent.Scope("child");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            
            var child = builder.Build().Container;
            var result = child.Resolve<string>();

            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }

        [Test]
        public void Resolve_OverrideParentInstallInChildAndResolveParent_ResolveParentValue()
        {
            var parent = Resolve
                .FromInstance(ContainerTestsConsts.CORRECT_INPUT)
                .BuildContainer("parent")
                .Container;

            var unused = Resolve
                .FromInstance(ContainerTestsConsts.WRONG_INPUT)
                .BuildContainer("child", parent)
                .Container;

            var result = parent.Resolve<string>();
            
            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }
    }
}
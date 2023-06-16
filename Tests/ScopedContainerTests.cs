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
            
            var parent = builder.Build().Container;

            builder = parent.Scope("child");
            builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
            
            var child = builder.Build().Container;
            string result = default;
            yield return child
                .Resolve<string>()
                .ToCoroutine(r => result = r);

            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }

        [UnityTest]
        public IEnumerator Resolve_OverrideParentInstallInChildAndResolveParent_ResolveParentValue()
        {
            var parent = Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).BuildContainer("parent").Container;

            var unused = Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT).BuildContainer("child", parent).Container;

            string result = null;
            yield return parent.Resolve<string>().ToCoroutine(r => result = r);
            Assert.AreEqual(result, ContainerTestsConsts.CORRECT_INPUT);
        }
    }
}
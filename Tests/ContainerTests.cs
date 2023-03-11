using System.Collections.Generic;
using NUnit.Framework;

namespace NotFluffy.NoFluffDI.Tests
{
	
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
			IContainer direct;
			IContainer parent;
			IContainer child;
				
			//FromInstance
			direct = new Container("direct");
			direct.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return direct;

			parent = new Container("parentWithCorrectInstances");
			parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			child = parent.Scope("childOfParentWithCorrectInstances");
			yield return child;
				
			parent = new Container("parent");
			child = parent.Scope("childWithInstances");
			child.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return child;
				
			parent = new Container("parentWithWrongInstances");
			parent.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.WRONG_INPUT).WithID(id));
			child = parent.Scope("childOfParentWithWrongInstances");
			child.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return child;
				
			//FromMethod
			direct = new Container("direct");
			direct.InstallSingle(Resolve.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return direct;

			parent = new Container("parentWithCorrectInstances");
			parent.InstallSingle(Resolve.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			child = parent.Scope("childOfParentWithCorrectInstances");
			yield return child;
				
			parent = new Container("parent");
			child = parent.Scope("childWithInstances");
			child.InstallSingle(Resolve.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return child;
				
			parent = new Container("parentWithWrongInstances");
			parent.InstallSingle(Resolve.FromMethod(() => ContainerTestsConsts.WRONG_INPUT).WithID(id));
			child = parent.Scope("childOfParentWithWrongInstances");
			child.InstallSingle(Resolve.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT).WithID(id));
			yield return child;
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
		
		[TestCaseSource(nameof(ContainersForResolve))]
		public void Resolve_StringFromInstalledContainer_True(IReadOnlyContainer container)
		{
			Assert.AreEqual(container.Resolve<string>(), ContainerTestsConsts.CORRECT_INPUT);
		}

		[TestCaseSource(nameof(ContainersForResolve))]
		public void Resolve_IntFromNotInstalledContainer_NoMatchingResolverException(IReadOnlyContainer container)
		{
			Assert.Throws<NoMatchingResolverException>(() => container.Resolve<int>());
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
		
		[TestCaseSource(nameof(ContainersForResolveWithID))]
		public void Resolve_StringFromInstalledContainerWithId_True(IReadOnlyContainer container)
		{
			Assert.AreEqual(container.Resolve<string>(ContainerTestsConsts.ID), ContainerTestsConsts.CORRECT_INPUT);
		}
		
		[TestCaseSource(nameof(ContainersForResolveWithID))]
		public void Resolve_StringFromInstalledContainerWithWrongId_NoMatchingResolverException(IReadOnlyContainer container)
		{
			Assert.Throws<NoMatchingResolverException>(() => container.Resolve<string>(ContainerTestsConsts.WRONG_ID));
		}

		[TestCaseSource(nameof(ContainersForResolveWithID))]
		public void Resolve_IntFromNotInstalledContainerWithId_NoMatchingResolverException(IReadOnlyContainer container)
		{
			Assert.Throws<NoMatchingResolverException>(() => container.Resolve<int>());
		}

		[Test]
		public void Resolve_AsTransient_ShouldAlwaysReturnANewInstance()
		{
			var container = new Container("container"); 
			container.InstallSingle(Resolve.FromNew<TypeWithState>().AsTransient());
			container.Resolve<TypeWithState>().State = ContainerTestsConsts.WRONG_INPUT;
			Assert.AreNotEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.WRONG_INPUT);
		}

		[Test]
		public void Resolve_AsSingle_ShouldAlwaysReturnSameInstance()
		{
			var container = new Container("container");
			container.InstallSingle(Resolve.FromNew<TypeWithState>().AsSingle());
			container.Resolve<TypeWithState>().State = ContainerTestsConsts.CORRECT_INPUT;
			Assert.AreEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.CORRECT_INPUT);
		}

		[Test]
		public void Resolve_FromMethod_ShouldExecuteMethod()
		{
			var container = new Container("container");
			container.InstallSingle(Resolve.FromMethod(() => new TypeWithState { State = ContainerTestsConsts.CORRECT_INPUT }));
			Assert.AreEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.CORRECT_INPUT);
		}

		[Test]
		public void Resolve_ResolveFromInsideResolve_CorrectlyResolve()
		{
			var container = new Container("container");
			container.InstallSingle(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
			container.InstallSingle(Resolve.FromMethod(() => new TypeWithState { State = container.Resolve<string>()}).AsTransient());
			Assert.AreEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.CORRECT_INPUT);
		}

		//Lazy/None lazy
		[Test]
		public void Resolve_InstallAsLazyAndDoesntResolve_ShouldNotResolve()
		{
			var wasResolved = false;
			using var container = new Container("");
			container.InstallSingle(Resolve.FromMethod(GetValue).Lazy());
			Assert.False(wasResolved);

			int GetValue()
			{
				wasResolved = true;
				return 5;
			}
		}

		[Test]
		public void Resolve_InstallAsLazyAndResolve_ShouldResolve()
		{
			var wasResolved = false;
			using var container = new Container("");
			container.InstallSingle(Resolve.FromMethod(GetValue).Lazy());
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
            using var container = new Container("");
            container.InstallSingle(Resolve.FromMethod(GetValue).NonLazy());
            Assert.True(wasResolved);

            int GetValue()
            {
	            wasResolved = true;
                return 5;
            }
        }
	}
}
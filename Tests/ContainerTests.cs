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
			IReadOnlyContainer parent;

			//FromInstance
			yield return Resolve
				.FromInstance(ContainerTestsConsts.CORRECT_INPUT)
				.WithID(id)
				.CreateContainer("direct");

			parent = Resolve
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
				.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT)
				.WithID(id)
				.CreateContainer("direct");

			parent = Resolve
				.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT)
				.WithID(id)
				.CreateContainer("parentWithCorrectInstances");
			
			yield return parent.Scope("childOfParentWithCorrectInstances").Build();

			parent = new ContainerBuilder("parent").Build();
			yield return Resolve
				.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT)
				.WithID(id)
				.CreateContainer("childWithInstances", parent);

			parent = Resolve
				.FromMethod(() => ContainerTestsConsts.WRONG_INPUT)
				.WithID(id)
				.CreateContainer("parentWithWrongInstances");
			
			yield return Resolve
				.FromMethod(() => ContainerTestsConsts.CORRECT_INPUT)
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
			var container = Resolve
				.FromNew<TypeWithState>()
				.AsTransient()
				.CreateContainer("container");
			
			container.Resolve<TypeWithState>().State = ContainerTestsConsts.WRONG_INPUT;
			Assert.AreNotEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.WRONG_INPUT);
		}

		[Test]
		public void Resolve_AsSingle_ShouldAlwaysReturnSameInstance()
		{
			var container = Resolve
				.FromNew<TypeWithState>()
				.AsSingle()
				.CreateContainer("container");
			
			container.Resolve<TypeWithState>().State = ContainerTestsConsts.CORRECT_INPUT;
			Assert.AreEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.CORRECT_INPUT);
		}

		[Test]
		public void Resolve_FromMethod_ShouldExecuteMethod()
		{
			var container = Resolve
				.FromMethod(() => new TypeWithState { State = ContainerTestsConsts.CORRECT_INPUT })
				.CreateContainer("container");
			
			Assert.AreEqual(container.Resolve<TypeWithState>().State, ContainerTestsConsts.CORRECT_INPUT);
		}

		[Test]
		public void Resolve_ResolveFromInsideResolve_CorrectlyResolve()
		{
			var builder = new ContainerBuilder("container");
			builder.Add(Resolve.FromInstance(ContainerTestsConsts.CORRECT_INPUT));
			builder.Add(Resolve.FromMethod(resolver => new TypeWithState { State = resolver.Resolve<string>()}).AsTransient());
			var container = builder.Build();
			var output = container.Resolve<TypeWithState>().State;
			
			Assert.AreEqual(output, ContainerTestsConsts.CORRECT_INPUT);
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
			var builder = new ContainerBuilder("");
			builder.Add(Resolve.FromMethod(GetValue).Lazy());

			var container = builder.Build();
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
            var builder = new ContainerBuilder("");
            builder.Add(Resolve.FromMethod(GetValue).NonLazy());

            builder.Build();
            
            Assert.True(wasResolved);

            int GetValue()
            {
	            wasResolved = true;
                return 5;
            }
        }
	}
}
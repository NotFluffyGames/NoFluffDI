﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NotFluffy.NoFluffRx;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.NoFluffDI
{

    public delegate UniTask Inject(IInjectContext context);
    
    public interface IContainerBuilder
    {
        /// <summary>
        /// Installs new binds into the container
        /// </summary>
        /// <param name="resolverFactory"></param>
        IContainerBuilder Add(IResolverFactory resolverFactory);
        
        IContainerBuilder AddInjectable(Inject injectable);

        IReadOnlyContainer Build();
        
        IContainerBuilder RegisterBuildCallback(Action<IReadOnlyContainer> container);
        
        IContainerBuilder RegisterInjectCallback(Action<IReadOnlyContainer> container);
    }

    /// <summary>
    /// Each container is a node in a tree of containers, each holds his local resolvers
    /// </summary>
    public interface IReadOnlyContainer : IReadOnlyReactiveDisposable
    {
        IReadOnlyContainer Parent { get; }
        /// <summary>
        /// The context can be used to identify the container, mainly used for debugging and editor tools
        /// </summary>
        object Context { get; }
        
        IContainerBuilder Scope(object context);
        
        /// <summary>
        /// The local resolvers inside the container, doesn't take into account the resolvers in the parents
        /// </summary>
        IReadOnlyDictionary<ResolverID, IResolver> Resolvers { get; }

        /// <summary>
        /// Resolves an instance from a resolver installed with the same type and id 
        /// </summary>
        /// <param name="contract">The type to resolve</param>
        /// <param name="id">If provided, will only use resolvers installed with the same id</param>
        /// <returns>An instance of the type requested</returns>
        UniTask<object> Resolve(Type contract, object id = null);

        /// <summary>
        /// Whether the type with the same id can be resolved, either directly or if the id is null then implicitly through a converter.
        /// </summary>
        /// <returns>Whether the type can resolved</returns>
        bool Contains(Type contract, object id = null);
        
        /// <summary>
        /// Finishes when all injections declared in the ContainerBuilder complete injection
        /// </summary>
        UniTask InjectionTask { get; }
    }
}
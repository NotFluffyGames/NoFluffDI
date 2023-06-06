﻿using System;
using System.Collections.Generic;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{

    public interface IContainerBuilder
    {
        /// <summary>
        /// Installs new binds into the container
        /// </summary>
        /// <param name="resolverFactories"></param>
        IContainerBuilder Add(IResolverFactory resolverFactory);
        IContainerBuilder SetImplicitConverter<TFrom, TTo>(Converter<TFrom, TTo> converter);

        IReadOnlyContainer Build();

        void RegisterBuildCallback(Action<IReadOnlyContainer> container);
    }

    /// <summary>
    /// Each container is a node in a tree of containers, each holds his local resolvers
    /// </summary>
    public interface IReadOnlyContainer : 
        //ITreeNode<IReadOnlyContainer>,
        IReadOnlyReactiveDisposable
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
        /// Converters are used when no direct resolver was found to implicitly resolve a type from a different type resolver without exposing the conversion to the user.
        /// each type can be converted into multiple other types, but can only be converted from one type (one to many).
        /// </summary>
        IReadOnlyDictionary<Type, ConverterBind> ImplicitConverters { get; }

        /// <summary>
        /// Resolves an instance from a resolver installed with the same type and id 
        /// </summary>
        /// <param name="contract">The type to resolve</param>
        /// <param name="id">If provided, will only use resolvers installed with the same id</param>
        /// <returns>An instance of the type requested</returns>
        object Resolve(Type contract, object id = null);

        /// <summary>
        /// Whether the type with the same id can be resolved, either directly or if the id is null then implicitly through a converter.
        /// </summary>
        /// <returns>Whether the type can resolved</returns>
        bool Contains(Type contract, object id = null);
        
        ConverterBind GetConverter(Type from, Type to);
    }
}
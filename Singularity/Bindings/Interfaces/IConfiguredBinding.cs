﻿using System;
using System.Linq.Expressions;


namespace Singularity.Bindings
{
	internal interface IConfiguredBinding
	{
        /// <summary>
        /// The expression that is used to instantiate the dependency instance
        /// </summary>
		Expression Expression { get; }

        /// <summary>
        /// The lifetime of the dependency
        /// </summary>
        /// <seealso cref="Enums.Lifetime"/>
		ILifetime Lifetime { get; }

        /// <summary>
        /// A delegate that will be called when the dependency goes out of scope
        /// </summary>
        Action<object>? OnDeath { get; }
	}
}
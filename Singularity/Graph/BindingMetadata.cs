using Singularity.Collections;
using System;

namespace Singularity.Graph
{
    /// <summary>
    /// Contains useful metadata of a registered binding such as on what line its registered.
    /// Is used to provide more info in error messages
    /// </summary>
	public sealed class BindingMetadata
    {
        /// <summary>
        /// The file path of the file in which the binding was registered.
        /// </summary>
		public string CreatorFilePath { get; }

        /// <summary>
        /// The line number in the file in which the binding was registered.
        /// </summary>
		public int CreatorLineNumber { get; }

        /// <summary>
        /// The module in which the binding was registered.
        /// </summary>
		public Type? ModuleType { get; }

        /// <summary>
        /// Is this binding dynamically generated or not?
        /// </summary>
        public bool Generated { get; }

        internal BindingMetadata(string creatorFilePath, int creatorLineNumber, IModule? module)
        {
            ModuleType = module?.GetType();
            CreatorFilePath = creatorFilePath;
            CreatorLineNumber = creatorLineNumber;
        }

        internal BindingMetadata()
        {
            Generated = true;
        }
    }
}
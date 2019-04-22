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

        /// <summary>
        /// The dependency type
        /// </summary>
        public Type[] DependencyType { get; }

        internal BindingMetadata(Type[] dependencyType, string creatorFilePath, int creatorLineNumber, IModule? module)
        {
            DependencyType = dependencyType;
            ModuleType = module?.GetType();
            CreatorFilePath = creatorFilePath;
            CreatorLineNumber = creatorLineNumber;
        }

        internal BindingMetadata(Type[] type)
        {
            Generated = true;
            DependencyType = type;
            CreatorFilePath = string.Empty;
            CreatorLineNumber = -1;
        }

        internal string StringRepresentation()
        {
            if (Generated)
            {
                return $"dynamically generated binding {DependencyType}";
            }
            return ModuleType == null ?
                $"registered binding in {CreatorFilePath} at line {CreatorLineNumber}" :
                $"registered binding in module {ModuleType.FullName} at line {CreatorLineNumber}";
        }
    }
}
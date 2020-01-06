using System;

namespace Singularity
{
    /// <summary>
    /// Contains useful metadata of a registered binding such as on what line its registered.
    /// Is used to provide more info in error messages
    /// </summary>
	public readonly struct BindingMetadata
    {
        internal static BindingMetadata GeneratedInstance = new BindingMetadata(-1);

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
            Generated = false;
        }

        private BindingMetadata(double dummy)
        {
            CreatorFilePath = "Generated";
            CreatorLineNumber = -1;
            ModuleType = null;
            Generated = true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Generated)
            {
                return "generated";
            }
            else if (ModuleType != null)
            {
                return ModuleType.AssemblyQualifiedName;
            }
            else
            {
                return $"file: {CreatorFilePath}, line {CreatorLineNumber}";
            }
        }
    }
}
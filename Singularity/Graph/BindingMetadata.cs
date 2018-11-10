using System;
using Singularity.Bindings;

namespace Singularity.Graph
{
	public sealed class BindingMetadata
	{
		public string CreatorFilePath { get; }
		public int CreatorLineNumber { get; }
		public Type ModuleType { get; }

		internal BindingMetadata(string creatorFilePath, int creatorLineNumber, IModule module)
		{
			ModuleType = module?.GetType();
			CreatorFilePath = creatorFilePath;
			CreatorLineNumber = creatorLineNumber;
		}

		public string GetPosition()
		{
			return ModuleType == null ?
				$"{CreatorFilePath} at line {CreatorLineNumber}" :
				$"module {ModuleType.FullName} at line {CreatorLineNumber}";
		}
	}
}
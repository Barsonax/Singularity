using System;
using Singularity.Bindings;

namespace Singularity.Graph
{
	public class BindingMetadata
	{
		public string CreatorFilePath { get; }
		public int CreatorLineNumber { get; }
		public Type ModuleType { get; }

		public BindingMetadata(string creatorFilePath, int creatorLineNumber, IModule module)
		{
			ModuleType = module?.GetType();
			CreatorFilePath = creatorFilePath;
			CreatorLineNumber = creatorLineNumber;
		}

		public string GetPosition()
		{
			if (ModuleType == null)
			{
				return $"{CreatorFilePath} at line {CreatorLineNumber}";
			}
			return $"module {ModuleType.FullName} at line {CreatorLineNumber}";
		}
	}
}
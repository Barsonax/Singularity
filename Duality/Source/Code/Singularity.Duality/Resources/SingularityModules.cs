using Duality;

namespace Singularity.Duality.Resources
{
	public sealed class SingularityModules : Resource
	{
		public ModuleRef[] Modules { get; set; } = new ModuleRef[1];
	}
}
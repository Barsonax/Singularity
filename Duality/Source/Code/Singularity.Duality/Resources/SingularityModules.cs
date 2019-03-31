using Duality;

namespace Singularity.Duality.Resources
{
    /// <summary>
    /// A resource to store <see cref="IModule"/>
    /// </summary>
	public sealed class SingularityModules : Resource
	{
        /// <summary>
        /// The modules.
        /// </summary>
		public ModuleRef[] Modules { get; set; } = new ModuleRef[1];
	}
}
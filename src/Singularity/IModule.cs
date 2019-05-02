namespace Singularity
{
    /// <summary>
    /// The interface for modules that can register dependencies.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Implement this method to register your dependencies
        /// </summary>
        /// <param name="config"></param>
	    void Register(ContainerBuilder config);
    }
}

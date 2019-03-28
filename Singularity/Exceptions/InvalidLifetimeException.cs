using Singularity.Bindings;

namespace Singularity.Exceptions
{
	public sealed class InvalidLifetimeException : SingularityException
	{
		internal InvalidLifetimeException(WeaklyTypedBinding unresolvedDependency) : base($"Registered binding in {unresolvedDependency.BindingMetadata.GetPosition()} has a invalid creationMode value of {unresolvedDependency.CreationMode}")
		{

		}
	}
}
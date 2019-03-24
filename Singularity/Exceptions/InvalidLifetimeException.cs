using Singularity.Graph;

namespace Singularity.Exceptions
{
	public sealed class InvalidLifetimeException : SingularityException
	{
		internal InvalidLifetimeException(Binding unresolvedDependency) : base($"Registered binding in {unresolvedDependency.BindingMetadata.GetPosition()} has a invalid lifetime value of {unresolvedDependency.Lifetime}")
		{

		}
	}
}
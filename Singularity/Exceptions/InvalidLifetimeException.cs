using Singularity.Graph;

namespace Singularity.Exceptions
{
	public class InvalidLifetimeException : SingularityException
	{
		public InvalidLifetimeException(UnresolvedDependency unresolvedDependency) : base($"Registered binding in {unresolvedDependency.BindingMetadata.GetPosition()} has a invalid lifetime value of {unresolvedDependency.Lifetime}")
		{

		}
	}
}
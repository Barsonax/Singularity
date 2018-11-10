namespace Singularity.Exceptions
{
	public sealed class CannotAutoResolveConstructorException : SingularityException
	{
        internal CannotAutoResolveConstructorException(string message) : base(message)
        {
        }
    }
}
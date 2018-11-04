namespace Singularity.Exceptions
{
	public class CannotAutoResolveConstructorException : SingularityException
	{
        public CannotAutoResolveConstructorException(string message) : base(message)
        {
        }
    }
}
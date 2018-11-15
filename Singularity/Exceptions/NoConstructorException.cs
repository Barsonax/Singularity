namespace Singularity.Exceptions
{
    public sealed class NoConstructorException : SingularityException
    {
        internal NoConstructorException(string message) : base(message)
        {
        }
    }
}
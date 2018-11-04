namespace Singularity.Duality.Scopes
{
	public interface ILogger
	{
		void WriteWarning(string text);
		void WriteError(string text);
	}
}
namespace Singularity.Duality.Scopes
{
	internal interface ILogger
	{
		void WriteWarning(string text);
		void WriteError(string text);
	}
}
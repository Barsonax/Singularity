using Duality;

namespace Singularity.Duality.Scopes
{
	internal sealed class LoggerAdapter : ILogger
	{
		private readonly Log _log;

		public LoggerAdapter(Log log)
		{
			_log = log;
		}

		public void WriteWarning(string text)
		{
			_log.WriteWarning(text);
		}

		public void WriteError(string text)
		{
			_log.WriteError(text);
		}
	}
}
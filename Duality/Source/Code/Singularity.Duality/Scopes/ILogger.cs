using Duality;

namespace Singularity.Duality.Scopes
{
	public interface ILogger
	{
		void WriteWarning(string text);
	}

	public class LoggerAdapter : ILogger
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
	}
}
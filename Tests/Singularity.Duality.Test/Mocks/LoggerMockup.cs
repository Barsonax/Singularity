using System.Collections.Generic;
using Singularity.Duality.Scopes;
using Singularity.Exceptions;

namespace Singularity.Duality.Test
{
	public class LoggerMockup : ILogger
	{
		public List<string> Warnings = new List<string>();
		public List<string> Errors = new List<string>();

		public void WriteWarning(string text)
		{
			Warnings.Add(text);
		}

		public void WriteError(string text)
		{
			Errors.Add(text);
		}
	}
}

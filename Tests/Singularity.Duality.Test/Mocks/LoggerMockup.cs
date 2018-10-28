using System.Collections.Generic;
using Singularity.Duality.Scopes;

namespace Singularity.Duality.Test
{
	public class LoggerMockup : ILogger
	{
		public List<string> Warnings = new List<string>();

		public void WriteWarning(string text)
		{
			Warnings.Add(text);
		}
	}
}

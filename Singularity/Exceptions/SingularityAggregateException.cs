using System;
using System.Collections.Generic;
using System.Linq;

namespace Singularity
{
	public class SingularityAggregateException : AggregateException
	{
		public string HeaderMessage { get; }
		public override string Message => $"{HeaderMessage}:{Environment.NewLine}{string.Join(Environment.NewLine, InnerExceptions.Select(x => x.Message))}";

		public SingularityAggregateException(string message, IEnumerable<Exception> innerExceptions) : base(innerExceptions)
		{
			HeaderMessage = message;
		}
	}
}
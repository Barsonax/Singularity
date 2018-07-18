using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Singularity
{
	public class SingularityAggregateException : AggregateException
	{
		public string HeaderMessage { get; }
		public override string Message => GenerateString(new StringBuilder(), 0, this).ToString();

		private static StringBuilder GenerateString(StringBuilder builder, int indentLevel, AggregateException exception)
		{
			if (exception is SingularityAggregateException singularityAggregateException)
			{
				builder.Append(new string('	', indentLevel));
				builder.AppendLine(singularityAggregateException.HeaderMessage);
			}
			indentLevel++;
			foreach (var innerException in exception.InnerExceptions)
			{


				if (innerException is AggregateException aggregateException)
				{
					GenerateString(builder, indentLevel, aggregateException);
				}
				else
				{					
					foreach (var line in innerException.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
					{
						builder.Append(new string('	', indentLevel));
						builder.AppendLine(line);
					}					
				}
			}

			return builder;
		}

		public SingularityAggregateException(string message, IEnumerable<Exception> innerExceptions) : base(innerExceptions)
		{
			HeaderMessage = message;


		}
	}
}
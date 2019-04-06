using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Singularity.Exceptions
{
    [Serializable]
    public class SingularityAggregateException : AggregateException
	{
		public string HeaderMessage { get; }
		public override string Message => GenerateString(new StringBuilder(), 0, this).ToString();

		private static StringBuilder GenerateString(StringBuilder builder, int indentLevel, AggregateException exception)
		{
			if (exception is SingularityAggregateException singularityAggregateException)
			{
				builder.Append(new string(' ', indentLevel * 2));
				builder.AppendLine(singularityAggregateException.HeaderMessage);
			}
			indentLevel++;
			foreach (Exception innerException in exception.InnerExceptions)
			{
				if (innerException is AggregateException aggregateException)
				{
					GenerateString(builder, indentLevel, aggregateException);
				}
				else
				{
					foreach (string line in innerException.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
					{
						builder.Append(new string(' ', indentLevel * 2));
						builder.AppendLine(line);
					}
				}
			}

			return builder;
		}

		internal SingularityAggregateException(string message, IEnumerable<Exception> innerExceptions) : base(innerExceptions)
		{
			HeaderMessage = message;
		}

        public SingularityAggregateException()
        {
        }

        protected SingularityAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
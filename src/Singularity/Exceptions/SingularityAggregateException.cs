using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Singularities version of the <see cref="AggregateException"/>. Has additional logic for formatting the message.
    /// </summary>
    [Serializable]
    public sealed class SingularityAggregateException : AggregateException
	{
        /// <summary>
        /// The header of this exception
        /// </summary>
		public string HeaderMessage { get; }

        /// <summary>
        /// The formatted message of this exception
        /// </summary>
		public override string Message => GenerateString(new StringBuilder(), 0, this).ToString();

        internal SingularityAggregateException(string message, IEnumerable<Exception> innerExceptions) : base(innerExceptions)
        {
            HeaderMessage = message;
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private SingularityAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            HeaderMessage = info.GetString(nameof(HeaderMessage));
        }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(HeaderMessage), HeaderMessage);
            base.GetObjectData(info, context);
        }

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
    }
}
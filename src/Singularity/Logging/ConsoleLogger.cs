using System;
using System.Diagnostics;

namespace Singularity.Logging
{
    /// <summary>
    /// Logger that logs to both console and debug.
    /// </summary>
    public class ConsoleLogger : ISingularityLogger
    {
        /// <inheritdoc />
        public void Log(string message, int indentLevel)
        {
            var indentString = new string(' ', indentLevel * 3);
            message = $"{indentString}{message}";
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
using System;
using System.Diagnostics;

namespace Singularity.Logging
{
    public class ConsoleLogger : ISingularityLogger
    {
        public void Log(string message, int indentLevel)
        {
            var indentString = new string(' ', indentLevel * 3);
            message = $"{indentString}{message}";
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
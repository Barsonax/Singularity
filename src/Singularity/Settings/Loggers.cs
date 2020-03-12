using Singularity.Logging;

namespace Singularity
{
    /// <summary>
    /// Utility class to easily used commonly used loggers.
    /// </summary>
    public class Loggers
    {
        /// <summary>
        /// The default logger that does nothing.
        /// </summary>
        public static NullLogger Default { get; } = new NullLogger();

        /// <summary>
        /// Logs to console and debug.
        /// </summary>
        public static ConsoleLogger ConsoleLogger { get; } = new ConsoleLogger();
    }
}

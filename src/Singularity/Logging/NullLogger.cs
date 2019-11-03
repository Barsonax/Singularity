namespace Singularity.Logging
{
    /// <summary>
    /// Logs nothing.
    /// </summary>
    public class NullLogger : ISingularityLogger
    {
        /// <inheritdoc />
        public void Log(string message, int indentLevel)
        {
            //noop
        }
    }
}
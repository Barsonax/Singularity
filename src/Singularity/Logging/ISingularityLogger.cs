namespace Singularity.Logging
{
    /// <summary>
    /// Generic Singularity Log interface
    /// </summary>
    public interface ISingularityLogger
    {
        /// <summary>
        /// Logs the value with the provided intentLevel
        /// </summary>
        void Log(string message, int indentLevel);
    }
}

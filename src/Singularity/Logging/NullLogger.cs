namespace Singularity.Logging
{
    public class NullLogger : ISingularityLogger
    {
        public void Log(string message, int indentLevel)
        {
            //noop
        }
    }
}
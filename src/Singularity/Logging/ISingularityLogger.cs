namespace Singularity.Logging
{
    public interface ISingularityLogger
    {
        void Log(string message, int indentLevel);
    }
}

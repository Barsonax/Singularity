using Xunit;

namespace Singularity.Test.Utils
{
    public class ReleaseOnlyFact : FactAttribute
    {
        public ReleaseOnlyFact()
        {
#if !RELEASE
            Skip = "Only running in release mode.";
#endif
        }
    }
}
using Singularity.Bindings;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding Binding { get; }
        public Dependency[]? Dependencies { get; set; }
        public ResolvedDependency? ResolvedDependency { get; set; }

        public Dependency(Binding unresolvedDependency)
        {
            Binding = unresolvedDependency;
        }
    }
}
using System;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding UnresolvedDependency { get; }
        public ResolvedDependency ResolvedDependency { get; }

        public Dependency(Binding unresolvedDependency, ResolvedDependency resolvedDependency)
        {
            UnresolvedDependency = unresolvedDependency ?? throw new ArgumentNullException(nameof(unresolvedDependency));
            ResolvedDependency = resolvedDependency ?? throw new ArgumentNullException(nameof(resolvedDependency));
        }
    }
}
using System;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public UnresolvedDependency UnresolvedDependency { get; }
        public ResolvedDependency ResolvedDependency { get; }

        public Dependency(UnresolvedDependency unresolvedDependency, ResolvedDependency resolvedDependency)
        {
            UnresolvedDependency = unresolvedDependency ?? throw new ArgumentNullException(nameof(unresolvedDependency));
            ResolvedDependency = resolvedDependency ?? throw new ArgumentNullException(nameof(resolvedDependency));
        }
    }
}
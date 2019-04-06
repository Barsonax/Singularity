using System;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding Binding { get; }
        public Dependency[]? Children { get; set; }
        public Dependency[]? Parents { get; set; }
        public Expression? Expression { get; set; }
        public Func<Scoped, object>? InstanceFactory { get; set; }
        public Exception ResolveError { get; set; }

        public Dependency(Binding unresolvedDependency)
        {
            Binding = unresolvedDependency;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding Binding { get; }
        public Dependency[]? Dependencies { get; set; }
        public Expression? Expression { get; set; }
        public Func<object>? InstanceFactory { get; set; }
        public Exception ResolveError { get; set; }

        public Dependency(Binding unresolvedDependency)
        {
            Binding = unresolvedDependency;
        }
    }
}
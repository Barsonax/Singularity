using System;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Collections;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public ReadonlyRegistration Registration { get; }
        public ArrayList<ResolvedDependency> ResolvedDependencies { get; }
        public ResolvedDependency Default { get; }

        public Dependency(ReadonlyRegistration registration)
        {
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
            ResolvedDependencies = new ArrayList<ResolvedDependency>();
            foreach (Binding binding in registration.Bindings)
            {
                ResolvedDependencies.Add(new ResolvedDependency(registration, binding));
            }

            Default = ResolvedDependencies.Array.LastOrDefault();
        }
    }

    internal sealed class ResolvedDependency
    {
        public ReadonlyRegistration Registration { get; }
        public Binding Binding { get; }
        public Dependency[]? Children { get; set; }
        public Expression? Expression { get; set; }
        public Func<Scoped, object>? InstanceFactory { get; set; }
        public Exception ResolveError { get; set; }

        public ResolvedDependency(ReadonlyRegistration registration, Binding binding)
        {
            Registration = registration;
            Binding = binding;
        }
    }
}
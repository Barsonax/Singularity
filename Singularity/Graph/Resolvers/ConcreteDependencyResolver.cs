using System;

namespace Singularity.Graph.Resolvers
{
    internal class ConcreteDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(DependencyGraph graph, Type type)
        {
            if (!type.IsInterface)
            {
                if (!graph.Dependencies.TryGetValue(type, out Dependency dependency))
                {
                    dependency = new Dependency(type, type.AutoResolveConstructorExpression(), CreationMode.Transient);
                    graph.Dependencies.Add(type, dependency);
                }
                return dependency;
            }

            return null;
        }
    }
}
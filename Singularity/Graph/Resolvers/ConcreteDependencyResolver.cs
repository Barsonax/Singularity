using System;

namespace Singularity.Graph.Resolvers
{
    internal class ConcreteDependencyResolver : IDependencyResolver
    {
        public Dependency? Resolve(DependencyGraph graph, Type type)
        {
            if (!type.IsInterface)
            {
                if (type.IsGenericType) return null;
                if (!graph.Dependencies.TryGetValue(type, out Dependency dependency))
                {
                    dependency = new Dependency(type, type.AutoResolveConstructorExpression(), CreationMode.Transient);
                }
                return dependency;
            }

            return null;
        }
    }
}
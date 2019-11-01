using System;

namespace Singularity.Graph.Resolvers
{
    public interface IResolverExclusion
    {
        bool IsExcluded(Type type);
    }
}
using System;

namespace Singularity.Graph.Resolvers
{
    public interface ITypeMatcher
    {
        bool Match(Type type);
    }
}
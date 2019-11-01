using System;

namespace Singularity.Graph.Resolvers
{
    public interface IMatch
    {
        bool Match(Type type);
    }
}
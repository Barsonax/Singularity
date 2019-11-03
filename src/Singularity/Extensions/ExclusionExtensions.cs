using System;
using System.Collections.Generic;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    public static class MatchExtensions
    {
        public static bool Match(this List<ITypeMatcher> exclusions, Type type)
        {
            foreach (ITypeMatcher x in exclusions)
            {
                if (x.Match(type)) return true;
            }

            return false;
        }
    }
}

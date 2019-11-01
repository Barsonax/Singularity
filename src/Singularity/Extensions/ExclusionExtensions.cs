using System;
using System.Collections.Generic;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    public static class MatchExtensions
    {
        public static bool Match(this List<IMatch> exclusions, Type type)
        {
            foreach (IMatch x in exclusions)
            {
                if (x.Match(type)) return true;
            }

            return false;
        }
    }
}

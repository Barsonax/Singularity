using System;
using System.Collections.Generic;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    /// <summary>
    /// Extensions for <see cref="ITypeMatcher"/>
    /// </summary>
    public static class MatchExtensions
    {
        /// <summary>
        /// Returns true if any of the matches match with the <paramref name="type"/>
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Match(this List<ITypeMatcher> matches, Type type)
        {
            foreach (ITypeMatcher x in matches)
            {
                if (x.Match(type)) return true;
            }

            return false;
        }
    }
}

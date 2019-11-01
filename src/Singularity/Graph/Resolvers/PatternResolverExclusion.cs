using System;

namespace Singularity.Graph.Resolvers
{
    public class PatternResolverExclusion : IResolverExclusion
    {
        private readonly string _pattern;

        public PatternResolverExclusion(string pattern)
        {
            _pattern = pattern;
        }

        public bool IsExcluded(Type type)
        {
            if (_pattern.StartsWith("*"))
            {
                return _pattern.EndsWith("*") ?
                    type.FullName.Contains(_pattern.Replace("*", "")) :
                    type.FullName.EndsWith(_pattern.Replace("*", ""));
            }
            else
            {
                return _pattern.EndsWith("*") ?
                    type.FullName.StartsWith(_pattern.Replace("*", "")) :
                    type.FullName == _pattern;
            }
        }
    }
}
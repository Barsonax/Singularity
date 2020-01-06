using System;

namespace Singularity
{
    /// <summary>
    /// Matches a type with a string.
    /// Wildcard supported.
    /// </summary>
    public class PatternTypeMatcher : ITypeMatcher
    {
        private readonly string _pattern;

        /// <summary>
        /// Creates a new pattern matcher.
        /// </summary>
        /// <param name="pattern"></param>
        public PatternTypeMatcher(string pattern)
        {
            _pattern = pattern;
        }

        /// <inheritdoc />
        public bool Match(Type type)
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
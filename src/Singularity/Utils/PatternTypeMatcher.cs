﻿using System;
using Singularity.Graph.Resolvers;

namespace Singularity
{
    public class PatternTypeMatcher : ITypeMatcher
    {
        private readonly string _pattern;

        public PatternTypeMatcher(string pattern)
        {
            _pattern = pattern;
        }

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
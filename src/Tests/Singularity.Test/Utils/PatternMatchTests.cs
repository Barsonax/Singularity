using System;
using System.Collections.Generic;
using System.Text;
using Singularity.Graph.Resolvers;
using Xunit;

namespace Singularity.Test.Utils
{
    public class PatternMatchTests
    {
        [Fact]
        public void Pattern_Match_Wildcard1_True()
        {
            var pattern = new List<ITypeMatcher> { new PatternTypeMatcher("Singularity.Test*") };

            var result = pattern.Match(typeof(PatternMatchTests));

            Assert.True(result);
        }

        [Fact]
        public void Pattern_Match_Wildcard2_True()
        {
            var pattern = new List<ITypeMatcher> { new PatternTypeMatcher("*PatternMatchTests*") };

            var result = pattern.Match(typeof(PatternMatchTests));

            Assert.True(result);
        }

        [Fact]
        public void Pattern_Match_True()
        {
            var pattern = new List<ITypeMatcher> { new PatternTypeMatcher("Singularity.Test.Utils.PatternMatchTests") };

            var result = pattern.Match(typeof(PatternMatchTests));

            Assert.True(result);
        }

        [Fact]
        public void Pattern_Match_False()
        {
            var pattern = new List<ITypeMatcher> { new PatternTypeMatcher("Foo") };

            var result = pattern.Match(typeof(PatternMatchTests));

            Assert.False(result);
        }
    }
}

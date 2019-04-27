using System;
using System.Collections.Generic;
using Xunit;

namespace Singularity.Test
{
    public class ContainerTests
    {
        [Fact]
        public void Container_NullModules_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<IModule>? modules = null;
                var container = new Container(modules!);
            });
        }
    }
}

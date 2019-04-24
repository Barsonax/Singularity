using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                IEnumerable<IModule> modules = null;
                var container = new Container(modules);
            });
        }

        [Fact]
        public void Container_NullBindings_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                BindingConfig bindingConfig = null;
                var container = new Container(bindingConfig);
            });
        }
    }
}

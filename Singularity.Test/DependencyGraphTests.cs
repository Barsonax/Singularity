using System;
using System.Collections.Generic;
using System.Text;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class DependencyGraphTests
    {
        [Fact]
        public void GetInstance_1Deep_PerCallLifetime()
        {
            var container = new Container();

            using (var builder = container.StartBuilding())
            {
                builder.Bind<ICompositeTestService13>().To<CompositeTestService13>();
                builder.Bind<ITestService10>().To<TestService10>();
                builder.Bind<ITestService11>().To<TestService11>();
                builder.Bind<ITestService12>().To<TestService12>();

                builder.Bind<ITestService20>().To<TestService20>();
                builder.Bind<ITestService21>().To<TestService21>();
                builder.Bind<ITestService22>().To<TestService22>();

                builder.Bind<ITestService30>().To<TestService30>();
                builder.Bind<ITestService31>().To<TestService31>();
                builder.Bind<ITestService32>().To<TestService32>();

                var graph = new DependencyGraph(builder.Bindings.Values);
            }
        }
    }
}

using System.Collections.Generic;

using Singularity.Resolvers.Generators;

using Xunit;

namespace Singularity.Test.Settings
{
    public class SingularitySettingsTests
    {
        [Fact]
        public void Replace_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.Replace(new List<IServiceBindingGenerator> { insertElement });

            var element = Assert.Single(settings.ServiceBindingGenerators);
            Assert.Equal(insertElement, element);
        }

        [Fact]
        public void Append_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.Append(insertElement);

            Assert.Equal(insertElement, settings.ServiceBindingGenerators[7]);
        }

        [Fact]
        public void Before_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.Before<OpenGenericBindingGenerator>(insertElement);

            Assert.Equal(insertElement, settings.ServiceBindingGenerators[6]);
        }

        [Fact]
        public void After_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.After<OpenGenericBindingGenerator>(insertElement);

            Assert.Equal(insertElement, settings.ServiceBindingGenerators[7]);
        }
    }
}

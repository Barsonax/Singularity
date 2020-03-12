using Singularity.Resolving.Generators;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Settings
{
    public class SingularitySettingsTests
    {
        [Fact]
        public void Replace_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var concreteServiceBindingGenerator = new ConcreteServiceBindingGenerator();

            settings.ConfigureServiceBindingGenerators(generators =>
            {
                generators.Replace(x => x is ContainerServiceBindingGenerator, concreteServiceBindingGenerator);
            });

            Assert.Equal(settings.ServiceBindingGenerators, new IServiceBindingGenerator[]
            {
                new ConcreteServiceBindingGenerator(),
                new CollectionServiceBindingGenerator(),
                new ExpressionServiceBindingGenerator(),
                new LazyServiceBindingGenerator(),
                new FactoryServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new OpenGenericBindingGenerator()
            }, new TypeEqualityComparer<IServiceBindingGenerator>());
        }

        [Fact]
        public void Remove_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            settings.ConfigureServiceBindingGenerators(generators =>
            {
                generators.Remove(x => x is ContainerServiceBindingGenerator);
            });

            Assert.Equal(settings.ServiceBindingGenerators, new IServiceBindingGenerator[]
            {
                new CollectionServiceBindingGenerator(),
                new ExpressionServiceBindingGenerator(),
                new LazyServiceBindingGenerator(),
                new FactoryServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new OpenGenericBindingGenerator()
            }, new TypeEqualityComparer<IServiceBindingGenerator>());
        }

        [Fact]
        public void Add_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.ConfigureServiceBindingGenerators(generators =>
            {
                generators.Add(insertElement);
            });

            Assert.Equal(settings.ServiceBindingGenerators, new IServiceBindingGenerator[]
            {
                new ContainerServiceBindingGenerator(),
                new CollectionServiceBindingGenerator(),
                new ExpressionServiceBindingGenerator(),
                new LazyServiceBindingGenerator(),
                new FactoryServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new OpenGenericBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
            }, new TypeEqualityComparer<IServiceBindingGenerator>());
        }

        [Fact]
        public void Before_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.ConfigureServiceBindingGenerators(generators =>
            {
                generators.Before(x => x is OpenGenericBindingGenerator, insertElement);
            });


            Assert.Equal(settings.ServiceBindingGenerators, new IServiceBindingGenerator[]
            {
                new ContainerServiceBindingGenerator(),
                new CollectionServiceBindingGenerator(),
                new ExpressionServiceBindingGenerator(),
                new LazyServiceBindingGenerator(),
                new FactoryServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new OpenGenericBindingGenerator(),
            }, new TypeEqualityComparer<IServiceBindingGenerator>());
        }

        [Fact]
        public void After_ServiceBindingGenerator()
        {
            var settings = SingularitySettings.Default;

            var insertElement = new ConcreteServiceBindingGenerator();
            settings.ConfigureServiceBindingGenerators(generators =>
            {
                generators.After(x => x is OpenGenericBindingGenerator, insertElement);
            });


            Assert.Equal(settings.ServiceBindingGenerators, new IServiceBindingGenerator[]
            {
                new ContainerServiceBindingGenerator(),
                new CollectionServiceBindingGenerator(),
                new ExpressionServiceBindingGenerator(),
                new LazyServiceBindingGenerator(),
                new FactoryServiceBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
                new OpenGenericBindingGenerator(),
                new ConcreteServiceBindingGenerator(),
            }, new TypeEqualityComparer<IServiceBindingGenerator>());
        }
    }
}

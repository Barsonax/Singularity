using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ChildContainerTests
    {
        [Fact]
        public void GetNestedContainer_OverrideInChild_Throws()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITransient1, Transient1>();
                builder.Register<ISingleton1, Singleton1>();
                builder.Register<ICombined1, Combined1>();
            });

            //ACT
            //ASSERT
            Assert.Throws<RegistrationAlreadyExistsException>(() =>
            {
                Container nestedContainer = container.GetNestedContainer(builder =>
                {
                    builder.Register<ITransient1, ScopedTransient>();
                });
            });
        }

        [Fact]
        public void Foo()
        {
            var container = new Container(builder =>
            {
                // Both registrations result in two initializations of WritingSingleton
                builder.Register<ISingleton1, Singleton1>(c => c.As<Singleton1>().With(Lifetimes.PerContainer));
                //builder.Register(typeof(WritingSingleton), c => c.As(typeof(ISingleton)).With(Lifetimes.PerContainer));
            });

            var childContainer = container.GetNestedContainer();

            var instance1 = childContainer.GetInstance<ISingleton1>();
            var instance2 = childContainer.GetInstance<Singleton1>();

            Assert.Equal(instance1, instance2);
        }
    }
}

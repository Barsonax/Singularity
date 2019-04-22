using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class CircularDependencyTests
    {
        [Fact]
        public void SimpleCircularDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ISimpleCircularDependency1, SimpleCircularDependency1>();
            config.Register<ISimpleCircularDependency2, SimpleCircularDependency2>();
            var container = new Container(config);

            //ACT
            //ASSERT
            var cycleError1 = Assert.Throws<CircularDependencyException>(() =>
            {
                var circularDependency = container.GetInstance<ISimpleCircularDependency1>();
            });
            Assert.Equal(3, cycleError1.Cycle.Length);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[0]);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError1.Cycle[1]);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[2]);

            var cycleError2 = Assert.Throws<CircularDependencyException>(() =>
            {
                var circularDependency = container.GetInstance<ISimpleCircularDependency2>();
            });
            Assert.Equal(3, cycleError1.Cycle.Length);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[0]);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError2.Cycle[1]);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[2]);
        }

        [Fact]
        public void ComplexCircularDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IComplexCircularDependency1, ComplexCircularDependency1>();
            config.Register<IComplexCircularDependency2, ComplexCircularDependency2>();
            config.Register<IComplexCircularDependency3, ComplexCircularDependency3>();
            var container = new Container(config);

            //ACT
            //ASSERT
            var cycleError1 = Assert.Throws<CircularDependencyException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency1>();
            });
            Assert.Equal(4, cycleError1.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError1.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError1.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[3]);

            var cycleError2 = Assert.Throws<CircularDependencyException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency2>();
            });
            Assert.Equal(4, cycleError2.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError2.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError2.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[3]);

            var cycleError3 = Assert.Throws<CircularDependencyException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency3>();
            });
            Assert.Equal(4, cycleError3.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError3.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError3.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError3.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError3.Cycle[3]);
        }
    }
}

﻿using Singularity.Exceptions;
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
            var container = new Container(builder =>
            {
                builder.Register<ISimpleCircularDependency1, SimpleCircularDependency1>();
                builder.Register<ISimpleCircularDependency2, SimpleCircularDependency2>();
            });

            //ACT
            //ASSERT
            var dependencyResolveException1 = Assert.Throws<DependencyResolveException>(() =>
            {
                var circularDependency = container.GetInstance<ISimpleCircularDependency1>();
            });
            var cycleError1 = Assert.IsType<CircularDependencyException>(dependencyResolveException1.InnerException?.InnerException?.InnerException);

            var dependencyResolveException2 = Assert.Throws<DependencyResolveException>(() =>
            {
                var circularDependency = container.GetInstance<ISimpleCircularDependency2>();
            });
            var cycleError2 = Assert.IsType<CircularDependencyException>(dependencyResolveException2.InnerException?.InnerException?.InnerException);


            Assert.Equal(3, cycleError1.Cycle.Length);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[0]);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError1.Cycle[1]);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[2]);

            Assert.Equal(3, cycleError2.Cycle.Length);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[0]);
            Assert.Equal(typeof(ISimpleCircularDependency1).AssemblyQualifiedName, cycleError2.Cycle[1]);
            Assert.Equal(typeof(ISimpleCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[2]);
        }

        [Fact]
        public void ComplexCircularDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IComplexCircularDependency1, ComplexCircularDependency1>();
                builder.Register<IComplexCircularDependency2, ComplexCircularDependency2>();
                builder.Register<IComplexCircularDependency3, ComplexCircularDependency3>();
            });

            //ACT
            //ASSERT
            var dependencyResolveException1 = Assert.Throws<DependencyResolveException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency1>();
            });
            var cycleError1 = Assert.IsType<CircularDependencyException>(dependencyResolveException1.InnerException?.InnerException?.InnerException?.InnerException);

            var dependencyResolveException2 = Assert.Throws<DependencyResolveException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency2>();
            });
            var cycleError2 = Assert.IsType<CircularDependencyException>(dependencyResolveException2.InnerException?.InnerException?.InnerException?.InnerException);

            var dependencyResolveException3 = Assert.Throws<DependencyResolveException>(() =>
            {
                var circularDependency = container.GetInstance<IComplexCircularDependency3>();
            });
            var cycleError3 = Assert.IsType<CircularDependencyException>(dependencyResolveException3.InnerException?.InnerException?.InnerException?.InnerException);

            Assert.Equal(4, cycleError1.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError1.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError1.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError1.Cycle[3]);


            Assert.Equal(4, cycleError2.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError2.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError2.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError2.Cycle[3]);


            Assert.Equal(4, cycleError3.Cycle.Length);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError3.Cycle[0]);
            Assert.Equal(typeof(IComplexCircularDependency1).AssemblyQualifiedName, cycleError3.Cycle[1]);
            Assert.Equal(typeof(IComplexCircularDependency2).AssemblyQualifiedName, cycleError3.Cycle[2]);
            Assert.Equal(typeof(IComplexCircularDependency3).AssemblyQualifiedName, cycleError3.Cycle[3]);
        }
    }
}

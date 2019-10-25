using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ResolveSelfTests
    {
        [Fact]
        public void GetInstance_ResolveContainer()
        {
            //ARRANGE
            var container = new Container();

            //ACT
            var resolvedContainer = container.GetInstance<Container>();

            //ASSERT
            Assert.Equal(container, resolvedContainer);
            Assert.Single(container.Registrations.Registrations);
        }

        [Fact]
        public void GetInstance_ResolveContainer_Child()
        {
            //ARRANGE
            var container = new Container();
            var childContainer = container.GetNestedContainer();

            //ACT
            var resolvedContainer = container.GetInstance<Container>();
            var resolvedChildContainer = childContainer.GetInstance<Container>();

            //ASSERT
            Assert.Equal(container, resolvedContainer);
            Assert.Equal(childContainer, resolvedChildContainer);

            Assert.NotEqual(container, childContainer);

            Assert.Single(container.Registrations.Registrations);
            Assert.Single(childContainer.Registrations.Registrations);
        }

        [Fact]
        public void GetInstance_ResolveDisposable_Child_Dispose()
        {
            //ARRANGE
            var container = new Container(c => { }, SingularitySettings.Microsoft);
            var childContainer = container.GetNestedContainer();

            //ACT
            var disposable = container.GetInstance<Disposable>();
            var disposableFromChild = childContainer.GetInstance<Disposable>();

            //ASSERT
            Assert.False(disposable.IsDisposed);
            Assert.False(disposableFromChild.IsDisposed);
            childContainer.Dispose();
            Assert.False(disposable.IsDisposed);
            Assert.True(disposableFromChild.IsDisposed);
        }

        [Fact]
        public void GetInstance_ResolveDisposable_Scope_Dispose()
        {
            //ARRANGE
            var container = new Container(c => { }, SingularitySettings.Microsoft);
            var scope = container.BeginScope();

            //ACT
            var disposable = container.GetInstance<Disposable>();
            var disposableFromScope = scope.GetInstance<Disposable>();

            //ASSERT
            Assert.False(disposable.IsDisposed);
            Assert.False(disposableFromScope.IsDisposed);
            scope.Dispose();
            Assert.False(disposable.IsDisposed);
            Assert.True(disposableFromScope.IsDisposed);
        }

        [Fact]
        public void GetInstance_ResolveScope()
        {
            //ARRANGE
            var container = new Container();

            //ACT
            var resolvedScope = container.GetInstance<Scoped>();

            //ASSERT
            Assert.Equal(container.ContainerScope, resolvedScope);
            Assert.Single(container.Registrations.Registrations);
        }

        [Fact]
        public void GetInstance_ResolveScope_Child()
        {
            //ARRANGE
            var container = new Container();
            var childContainer = container.GetNestedContainer();

            //ACT
            var resolvedScope = container.GetInstance<Scoped>();
            var resolvedChildScope = childContainer.GetInstance<Scoped>();

            //ASSERT
            Assert.Equal(container.ContainerScope, resolvedScope);
            Assert.Equal(childContainer.ContainerScope, resolvedChildScope);

            Assert.NotEqual(resolvedScope, resolvedChildScope);

            Assert.Single(container.Registrations.Registrations);
            Assert.Single(childContainer.Registrations.Registrations);
        }
    }
}

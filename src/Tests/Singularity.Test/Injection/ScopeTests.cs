﻿using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ScopeTests
    {
        [Fact]
        public void GetInstance_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>();
            });
            var scope = container.BeginScope();

            //ACT
            var value = scope.GetInstance<ITestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_ReturnsCorrectDependency()
        {
            //ARRANGE
            var container = new Container();
            var scope = container.BeginScope();

            //ACT
            var value = scope.GetInstance<TestService10>();

            //ASSERT
            Assert.IsType<TestService10>(value);
        }

        [Fact]
        public void GetInstanceOrDefault_Generic_NotRegistered_ReturnsNull()
        {
            //ARRANGE
            var container = new Container();
            var scope = container.BeginScope();

            //ACT
            var value = scope.GetInstanceOrDefault<ITestService10>();

            //ASSERT
            Assert.Null(value);
        }

        [Fact]
        public void GetInstanceOrDefault_NotRegistered_ReturnsNull()
        {
            //ARRANGE
            var container = new Container();
            var scope = container.BeginScope();

            //ACT
            var value = scope.GetInstanceOrDefault(typeof(ITestService10));

            //ASSERT
            Assert.Null(value);
        }

        [Fact]
        public void GetInstance_DisposeScope()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(Lifetimes.PerScope)
                    .WithFinalizer(x => x.Dispose()));
            });

            //ACT
            Scoped scope = container.BeginScope();
            var disposable = (Disposable)scope.GetInstance<IDisposable>();
            bool isDisposedBefore = disposable.IsDisposed;
            scope.Dispose();
            bool isDisposedAfter = disposable.IsDisposed;

            //ASSERT
            Assert.False(isDisposedBefore);
            container.Dispose();
            Assert.True(isDisposedAfter);
        }

        [Fact]
        public void GetInstance_AlwaysReturnsSameInstanceForTheSameScope()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IDisposable, Disposable>(c => c
                    .With(Lifetimes.PerScope)
                    .WithFinalizer(x => x.Dispose()));
            });

            //ACT
            Scoped scope1 = container.BeginScope();
            var disposable1 = scope1.GetInstance<IDisposable>();
            var disposable2 = scope1.GetInstance<IDisposable>();

            Scoped scope2 = container.BeginScope();
            var disposable3 = scope2.GetInstance<IDisposable>();
            var disposable4 = scope2.GetInstance<IDisposable>();


            //ASSERT
            Assert.Same(disposable1, disposable2);
            Assert.Same(disposable3, disposable4);
            Assert.NotSame(disposable1, disposable3);
        }

        [Fact]
        public void GetInstance_Nested()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<ITestService10, TestService10>(c => c
                    .With(Lifetimes.PerScope));
                builder.Register<ITestService11, TestService11>();
            });

            //ACT
            Scoped scope1 = container.BeginScope();
            var testService11 = scope1.GetInstance<ITestService11>();

            //ASSERT
            Assert.IsType<TestService11>(testService11);
            Assert.IsType<TestService10>(testService11.TestService10);
        }

        [Fact]
        public void HandleScopeThreadCollision_AddsInstance_Once()
        {
            //ARRANGE
            var scope = new Scoped(new Container());
            var instance = new object();

            //ACT
            scope.HandleScopeThreadCollision(instance, typeof(object));
            var result = scope.GetOrAddScopedInstance<object>(scoped => throw new NotImplementedException(), typeof(object));

            //ASSERT
            Assert.Same(instance, result);
        }

        [Fact]
        public void HandleScopeThreadCollision_AddsInstance_Twice_SameKey()
        {
            //ARRANGE
            var scope = new Scoped(new Container());
            var instance = new object();

            //ACT
            scope.HandleScopeThreadCollision(instance, typeof(object));
            scope.HandleScopeThreadCollision(instance, typeof(object));
            var result = scope.GetOrAddScopedInstance<object>(scoped => throw new NotImplementedException(), typeof(object));

            //ASSERT
            Assert.Same(instance, result);
        }

        [Fact]
        public void HandleScopeThreadCollision_AddsInstance_Twice_DifferentKey()
        {
            //ARRANGE
            var scope = new Scoped(new Container());
            var instance1 = new object();
            var instance2 = new object();

            //ACT
            scope.HandleScopeThreadCollision(instance1, typeof(object));
            scope.HandleScopeThreadCollision(instance2, typeof(int));
            var result1 = scope.GetOrAddScopedInstance<object>(scoped => throw new NotImplementedException(), typeof(object));
            var result2 = scope.GetOrAddScopedInstance<object>(scoped => throw new NotImplementedException(), typeof(int));

            //ASSERT
            Assert.Same(instance1, result1);
            Assert.Same(instance2, result2);
        }
    }
}

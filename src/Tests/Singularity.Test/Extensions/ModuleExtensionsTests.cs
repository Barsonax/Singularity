﻿using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Extensions
{
	public class ModuleExtensionsTests
	{
		[Fact]
		public void ToBindings_SingleModule()
		{
			var modules = new[] {new TestModule1(),};
			var container = new Container(modules);
			var instance = container.GetInstance<ITestService10>();
			Assert.IsType<TestService10>(instance);
		}

        [Fact]
        public void RegisterModule_Generic()
        {
            //ARRANGE
            var container = new Container(cb =>
            {
                //ACT
                cb.RegisterModule<TestModule1>();
            });

            //ASSERT
            var instance = container.GetInstance<ITestService10>();
            Assert.IsType<TestService10>(instance);
        }
	}
}

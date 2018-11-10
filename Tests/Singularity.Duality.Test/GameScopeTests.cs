using System;

using NSubstitute;
using Singularity.Bindings;
using Singularity.Duality.Resources;
using Singularity.Duality.Scopes;
using Singularity.Duality.Test.Setup;
using Xunit;

namespace Singularity.Duality.Test
{
	[Collection(nameof(DualityCollection))]
	public class GameScopeTests
	{
		[Fact]
		public void Constructor_ModuleResourceWithNull_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResources = new[] { new SingularityModules(), };
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.Single(logger.Warnings);
			}
		}

		[Fact]
		public void Constructor_ModuleResourceInvalidType_Logs()
		{
			var logger = new LoggerMockup();
		    var moduleResource = new SingularityModules
		    {
		        Modules =
		        {
		            [0] = new ModuleRef()
		        }
		    };
		    SingularityModules[] moduleResources = new[] {moduleResource };
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.Single(logger.Warnings);
			}
		}

		[Fact]
		public void Constructor_ModuleResourceDoesntimplementIModule_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			Type type = typeof(GameScopeTests);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			SingularityModules[] moduleResources = new[] { moduleResource };
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.Single(logger.Warnings);
			}
		}

		[Fact]
		public void Constructor_ModuleResourceHasInvalidConstructor_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			Type type = typeof(TestModuleWithConstructor);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			SingularityModules[] moduleResources = new[] { moduleResource };
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.Single(logger.Warnings);
			}
		}

		[Fact]
		public void Constructor_ValidModuleResource_ReturnsDependency()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			Type type = typeof(TestModule);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			SingularityModules[] moduleResources = new[] { moduleResource };
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.Empty(logger.Warnings);
				var instance = scope.Container.GetInstance<IModule>();
				Assert.IsType<TestModule>(instance);
			}
		}

		[Fact]
		public void Disposed_ContainerIsDisposed()
		{
			var logger = new LoggerMockup();

			var moduleResources = new SingularityModules[0];
			using (var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), Substitute.For<ISceneEventsProvider>(), moduleResources))
			{
				Assert.False(scope.Container.IsDisposed);
				scope.Dispose();
				Assert.True(scope.Container.IsDisposed);
			}
		}

		[Fact]
		public void SwitchScene_SceneScopeIsCreated()
		{
			var logger = new LoggerMockup();
			var sceneScopeFactory = new SceneScopeFactoryMockup();

			var moduleResources = new SingularityModules[0];
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var scope = new GameScope(logger, sceneScopeFactory, sceneEventsProvider, moduleResources))
			{
				sceneEventsProvider.TriggerEntered();

				Assert.Single(sceneScopeFactory.CreateCalls);
			}
		}

		[Fact]
		public void SceneIsDisposed_SceneScopeIsDiposed()
		{
			var logger = new LoggerMockup();
			var sceneScopeFactory = new SceneScopeFactoryMockup();

			var moduleResources = new SingularityModules[0];
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var scope = new GameScope(logger, sceneScopeFactory, sceneEventsProvider, moduleResources))
			{
				sceneEventsProvider.TriggerEntered();

				SceneScope createdSceneScope = sceneScopeFactory.CreatedSceneScopes[0];
				Assert.False(createdSceneScope.IsDisposed);

				sceneEventsProvider.TriggerLeaving();
				Assert.True(createdSceneScope.IsDisposed);
			}
		}
	}
}

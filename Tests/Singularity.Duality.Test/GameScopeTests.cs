using Duality;
using Duality.Resources;
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
			var moduleResources = new[] { new ContentRef<SingularityModules>(new SingularityModules()), };
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.Single(logger.Warnings);
		}

		[Fact]
		public void Constructor_ModuleResourceInvalidType_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			moduleResource.Modules[0] = new ModuleRef();
			var moduleResources = new[] { new ContentRef<SingularityModules>(moduleResource) };
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.Single(logger.Warnings);
		}

		[Fact]
		public void Constructor_ModuleResourceDoesntimplementIModule_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			var type = typeof(GameScopeTests);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			var moduleResources = new[] { new ContentRef<SingularityModules>(moduleResource) };
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.Single(logger.Warnings);
		}

		[Fact]
		public void Constructor_ModuleResourceHasInvalidConstructor_Logs()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			var type = typeof(TestModuleWithConstructor);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			var moduleResources = new[] { new ContentRef<SingularityModules>(moduleResource) };
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.Single(logger.Warnings);
		}

		[Fact]
		public void Constructor_ValidModuleResource_ReturnsDependency()
		{
			var logger = new LoggerMockup();
			var moduleResource = new SingularityModules();
			var type = typeof(TestModule);

			moduleResource.Modules[0] = ModuleRef.FromType(type);
			var moduleResources = new[] { new ContentRef<SingularityModules>(moduleResource) };
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.Empty(logger.Warnings);
			var instance = scope.Container.GetInstance<IModule>();
			Assert.IsType<TestModule>(instance);
		}

		[Fact]
		public void Disposed_ContainerIsDisposed()
		{
			var logger = new LoggerMockup();

			var moduleResources = new ContentRef<SingularityModules>[0];
			var scope = new GameScope(logger, Substitute.For<ISceneScopeFactory>(), moduleResources);
			Assert.False(scope.Container.IsDisposed);
			scope.Dispose();
			Assert.True(scope.Container.IsDisposed);
		}

		[Fact]
		public void SwitchScene_SceneScopeIsCreated()
		{
			var logger = new LoggerMockup();
			var sceneScopeFactory = new SceneScopeFactoryMockup();

			var moduleResources = new ContentRef<SingularityModules>[0];
			var scope = new GameScope(logger, sceneScopeFactory, moduleResources);

			var scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));

			Assert.Single(sceneScopeFactory.CreateCalls);
		}

		[Fact]
		public void SceneIsDisposed_SceneScopeIsDiposed()
		{
			var logger = new LoggerMockup();
			var sceneScopeFactory = new SceneScopeFactoryMockup();

			var moduleResources = new ContentRef<SingularityModules>[0];
			var scope = new GameScope(logger, sceneScopeFactory, moduleResources);

			var scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));

			var createdSceneScope = sceneScopeFactory.CreatedSceneScopes[0];
			Assert.False(createdSceneScope.IsDisposed);
			scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));
			Assert.True(createdSceneScope.IsDisposed);
		}
	}
}

#if NETCOREAPP3_0
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Singularity.Test.Utils;
using Xunit;

namespace Singularity.Test.Injection
{
    public class MemoryTests
    {

        [Fact]
        public void GetInstance_DynamicallyCompiledType_IsGarbageCollected()
        {
            GarbageCollectionUtils.CheckIfCleanedUp(() =>
            {
                //ARRANGE
                var references = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(Binder).Assembly.Location),
                    MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName?.Substring(0, x.FullName.IndexOf(',')) == "netstandard").Location),
                };
                var parseOpts = new CSharpParseOptions(
                    kind: SourceCodeKind.Regular,
                    languageVersion: LanguageVersion.Latest);
                var options = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release);
                SyntaxTree csTree = CSharpSyntaxTree.ParseText("class A { }", parseOpts);
                Compilation compilation =
                    CSharpCompilation.Create(Guid.NewGuid().ToString(), new[] { csTree }, references, options);

                using var memoryStream = new MemoryStream();
                compilation.Emit(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var unloadableLoadContext = new AssemblyLoadContext(null, true);
                var weakRef = new WeakReference(unloadableLoadContext);

                Assembly assembly = unloadableLoadContext.LoadFromStream(memoryStream);
                var container = new Container();

                //ACT
                container.GetInstance(assembly.GetType("A")!);
                container.Dispose();
                unloadableLoadContext.Unload();

                //ASSERT
                return new CleanupTestSet(weakRef);
            });
        }

    }
}
#endif
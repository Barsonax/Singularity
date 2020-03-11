using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Singularity.Test.Utils;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class MemoryTests
    {
        [ReleaseOnlyFact]
        public void ScopedInstanceIsReleased()
        {
            CheckIfCleanedUp(() =>
            {
                //ARRANGE
                var container = new Container(c =>
                {
                    c.Register<Disposable>(s => s.With(ServiceAutoDispose.Always));
                });

                //ACT
                var weakRef = new WeakReference(container.GetInstance<Disposable>());
                container.Dispose();

                //ASSERT
                return weakRef;
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CheckIfCleanedUp(Func<WeakReference> func)
        {
            var weakRef = func.Invoke();

            for (int i = 0; weakRef.IsAlive && i < 10; i++)
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
            }

            Assert.False(weakRef.IsAlive);
        }

        class UnloadableLoadContext : AssemblyLoadContext
        {
            public UnloadableLoadContext() : base(true) { }
            protected override Assembly Load(AssemblyName assemblyName) => null;
        }

        static readonly CSharpParseOptions parseOpts = new CSharpParseOptions(
            kind: SourceCodeKind.Regular,
            languageVersion: LanguageVersion.Latest);
        static readonly CSharpCompilationOptions options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Release,
            allowUnsafe: false);

        [ReleaseOnlyFact]
        public void Foo()
        {
            CheckIfCleanedUp(() =>
            {
                var references = new List<MetadataReference>()
                {
                    MetadataReference.CreateFromFile(typeof(Binder).Assembly.Location),
                    MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Substring(0, x.FullName.IndexOf(',')) == "netstandard").Location),
                };

                var csTree = CSharpSyntaxTree.ParseText("class A { }", parseOpts);
                Compilation compilation = CSharpCompilation.Create(Guid.NewGuid().ToString(), options: options, references: references).AddSyntaxTrees(csTree);

                using var ms = new MemoryStream();
                compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var alc = new UnloadableLoadContext();
                var weakRef = new WeakReference(alc);

                var asm = alc.LoadFromStream(ms);
                using (var container = new Container(c =>
                {
                    c.ConfigureSettings(s =>
                    {
                        s.With(new DefaultConstructorResolver());
                    });
                }))
                {
                    var inst = container.GetInstance(asm.GetType("A"));
                }
                alc.Unload();

                return weakRef;
            });
        }
    }
}

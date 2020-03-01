using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var references = new List<MetadataReference>() {
                MetadataReference.CreateFromFile(typeof(Binder).Assembly.Location),
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Substring(0, x.FullName.IndexOf(',')) == "netstandard").Location),
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Substring(0, x.FullName.IndexOf(',')) == "System.Console").Location),
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Substring(0, x.FullName.IndexOf(',')) == "System.Runtime").Location)
            };

            var csTree = CSharpSyntaxTree.ParseText("class A { public void B() { System.Console.WriteLine(\"hello\"); } }", parseOpts);
            var asmName = Guid.NewGuid().ToString();
            Compilation compilation = CSharpCompilation.Create(asmName, options: options, references: references).AddSyntaxTrees(csTree);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var alc = new UnloadableLoadContext();
                var alcWeakRef = new WeakReference(alc);

                var asm = alc.LoadFromStream(ms);
                var mthd = asm.GetType("A").GetMethod("B");
                using (var container = new Container(c =>
                {
                    c.ConfigureSettings(s =>
                    {
                        s.With(new DefaultConstructorResolver());
                    });
                }))
                {
                    using (var scope = container.BeginScope())
                    {
                        var inst = container.GetInstance(asm.GetType("A"));

                        mthd.Invoke(inst, null);
                    }

                }
                alc.Unload();


                for (int i = 0; alcWeakRef.IsAlive && (i < 10); i++)
                {
                    GC.Collect(2, GCCollectionMode.Forced, true);
                    GC.WaitForPendingFinalizers();
                }

                Assert.False(alcWeakRef.IsAlive);
            }
        }
    }
}

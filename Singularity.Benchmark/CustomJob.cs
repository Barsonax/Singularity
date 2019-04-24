using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;

namespace Singularity.Benchmark
{
    public class CustomJob : ManualConfig
    {
        public CustomJob()
        {
            Add(
                Job.RyuJitX64
                    .With(Runtime.Core)
                    .WithIterationTime(TimeInterval.FromMilliseconds(150)));

            Add(
                Job.LegacyJitX64
                    .With(Runtime.Clr)
                    .WithIterationTime(TimeInterval.FromMilliseconds(150)));
        }
    }
}
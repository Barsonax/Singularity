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
                    .With(CoreRuntime.Core31)
                    .WithIterationTime(TimeInterval.FromMilliseconds(1000)));

            Add(
                Job.RyuJitX64
                    .With(CoreRuntime.Core21)
                    .WithIterationTime(TimeInterval.FromMilliseconds(1000)));

            Add(
                Job.LegacyJitX64
                    .With(ClrRuntime.Net472)
                    .WithIterationTime(TimeInterval.FromMilliseconds(1000)));
        }
    }
}
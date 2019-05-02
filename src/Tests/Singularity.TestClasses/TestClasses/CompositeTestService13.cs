namespace Singularity.TestClasses.TestClasses
{
    public interface ICompositeTestService13
    {
        ITestService10 TestService10 { get; }
        ITestService11 TestService11 { get; }
        ITestService12 TestService12 { get; }
    }

    public class CompositeTestService13 : ICompositeTestService13
    {
        public ITestService10 TestService10 { get; }
        public ITestService11 TestService11 { get; }
        public ITestService12 TestService12 { get; }

        public CompositeTestService13(ITestService10 testService10, ITestService11 testService11, ITestService12 testService12)
        {
            TestService10 = testService10;
            TestService11 = testService11;
            TestService12 = testService12;
        }
    }
}

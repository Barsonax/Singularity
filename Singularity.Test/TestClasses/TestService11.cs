namespace Singularity.Test.TestClasses
{
    public interface ITestService11
    {
        ITestService10 TestService10 { get; }
    }

    public class TestService11 : ITestService11
    {
        public ITestService10 TestService10 { get; }
        public TestService11(ITestService10 testService10)
        {
            TestService10 = testService10;
        }
    }
}

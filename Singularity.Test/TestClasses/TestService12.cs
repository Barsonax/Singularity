namespace Singularity.Test.TestClasses
{
    public interface ITestService12
    {
        ITestService11 TestService11 { get; }
    }

    public class TestService12 : ITestService12
    {
        public ITestService11 TestService11 { get; }
        public TestService12(ITestService11 testService11)
        {
            TestService11 = testService11;
        }
    }
}

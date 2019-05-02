namespace Singularity.TestClasses.TestClasses
{
    public interface ITestService21
    {
        ITestService20 TestService20 { get; }
    }

    public class TestService21 : ITestService21
    {
        public ITestService20 TestService20 { get; }
        public TestService21(ITestService20 testService20)
        {
            TestService20 = testService20;
        }
    }
}

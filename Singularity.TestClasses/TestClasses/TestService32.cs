namespace Singularity.TestClasses.TestClasses
{
    public interface ITestService32
    {
        ITestService31 TestService31 { get; }
    }

    public class TestService32 : ITestService32
    {
        public ITestService31 TestService31 { get; }
        public TestService32(ITestService31 testService31)
        {
            TestService31 = testService31;
        }
    }
}

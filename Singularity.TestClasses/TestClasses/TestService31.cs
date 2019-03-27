namespace Singularity.TestClasses.TestClasses
{
    public interface ITestService31
    {
        ITestService30 TestService30 { get; }
    }

    public class TestService31 : ITestService31
    {
        public ITestService30 TestService30 { get; }
        public TestService31(ITestService30 testService30)
        {
            TestService30 = testService30;
        }
    }
}

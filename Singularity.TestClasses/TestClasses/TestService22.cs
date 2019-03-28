namespace Singularity.TestClasses.TestClasses
{
    public interface ITestService22
    {
        ITestService21 TestService21 { get; }
    }

    public class TestService22 : ITestService22
    {
        public ITestService21 TestService21 { get; }
        public TestService22(ITestService21 testService21)
        {
            TestService21 = testService21;
        }
    }
}

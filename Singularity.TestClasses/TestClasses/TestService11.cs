namespace Singularity.TestClasses.TestClasses
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

	public class TestService11WithConcreteDependency : ITestService11
	{
		public TestService10 TestService10 { get; }

		ITestService10 ITestService11.TestService10 => TestService10;

		public TestService11WithConcreteDependency(TestService10 testService10)
		{
			TestService10 = testService10;
		}
	}
}

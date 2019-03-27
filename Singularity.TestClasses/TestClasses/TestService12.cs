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

	public class TestService12WithConcreteDependency : ITestService12
	{
		public TestService11WithConcreteDependency TestService11 { get; }

		ITestService11 ITestService12.TestService11 => TestService11;

		public TestService12WithConcreteDependency(TestService11WithConcreteDependency testService11)
		{
			TestService11 = testService11;
		}
	}

	public class TestService12WithMixedConcreteDependency : ITestService12
	{
		public ITestService11 TestService11 { get; }

		public TestService12WithMixedConcreteDependency(ITestService11 testService11)
		{
			TestService11 = testService11;
		}
	}
}

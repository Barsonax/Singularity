namespace Singularity.Test.TestClasses
{
	public class Decorator2 : IComponent
	{
		public IComponent Component { get; }

		public Decorator2(IComponent decoratee)
		{
			Component = decoratee;
		}
	}

	public class Decorator1 : IComponent
	{
		public IComponent Component { get; }

		public Decorator1(IComponent decoratee)
		{
			Component = decoratee;
		}
	}

    public class DecoratorWrongInterface : ITestService10
    {
        public IComponent Component { get; }

        public DecoratorWrongInterface(IComponent decoratee)
        {
            Component = decoratee;
        }
    }

    public class Component : IComponent { }

	public interface IComponent { }

    public class TestService11_Decorator1 : ITestService11
    {
        public ITestService10 TestService10 => TestService11.TestService10;
        public ITestService11 TestService11 { get; }

        public TestService11_Decorator1(ITestService11 testService11)
        {
            TestService11 = testService11;
        }
    }

    public class TestService11_Decorator2 : ITestService11
    {
        public ITestService10 TestService10 => TestService11.TestService10;
        public ITestService10 TestService10FromIOC { get; }
        public ITestService11 TestService11 { get; }

        public TestService11_Decorator2(ITestService11 testService11, ITestService10 testService10)
        {
            TestService11 = testService11;
            TestService10FromIOC = testService10;
        }
    }
}

namespace Singularity.TestClasses.TestClasses
{
	public class DecoratorWithNoInterface : TestService10
	{
		public TestService10 TestService10 { get; }

		public DecoratorWithNoInterface(DecoratorWithNoInterface decoratee)
		{
			TestService10 = decoratee;
		}
	}

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

    public class DecoratorWrongConstructorArguments : ITestService10
    {
        public IComponent Component { get; }

        public DecoratorWrongConstructorArguments(IComponent decoratee)
        {
            Component = decoratee;
        }
    }

    public class Component : IComponent { }

	public interface IComponent { }

	public class TestService10_Decorator1 : ITestService10
	{
		public ITestService10 TestService10 { get; }

		public TestService10_Decorator1(ITestService10 testService10, DummyOne dummyValue)
		{
			TestService10 = testService10;
		}
	}

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

namespace Singularity.TestClasses.TestClasses
{
	public interface ISimpleCircularDependency1
	{
		ISimpleCircularDependency2 circularDependency2 { get; }
	}

	public class SimpleCircularDependency1 : ISimpleCircularDependency1
	{
		public ISimpleCircularDependency2 circularDependency2 { get; }

		public SimpleCircularDependency1(ISimpleCircularDependency2 circularDependency2)
		{
			this.circularDependency2 = circularDependency2;
		}
	}

    public interface ISimpleCircularDependency2
    {
        ISimpleCircularDependency1 circularDependency1 { get; }
    }

    public class SimpleCircularDependency2 : ISimpleCircularDependency2
    {
        public ISimpleCircularDependency1 circularDependency1 { get; }

        public SimpleCircularDependency2(ISimpleCircularDependency1 circularDependency1)
        {
            this.circularDependency1 = circularDependency1;
        }
    }

    public interface IComplexCircularDependency1
    {

    }
    public class ComplexCircularDependency1 : IComplexCircularDependency1
    {
        public ComplexCircularDependency1(IComplexCircularDependency2 complexCircularDependency)
        {

        }
    }

    public interface IComplexCircularDependency2
    {

    }
    public class ComplexCircularDependency2 : IComplexCircularDependency2
    {
        public ComplexCircularDependency2(IComplexCircularDependency3 complexCircularDependency)
        {

        }
    }

    public interface IComplexCircularDependency3
    {

    }
    public class ComplexCircularDependency3 : IComplexCircularDependency3
    {
        public ComplexCircularDependency3(IComplexCircularDependency1 complexCircularDependency)
        {

        }
    }
}

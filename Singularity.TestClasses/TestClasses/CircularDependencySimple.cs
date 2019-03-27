namespace Singularity.Test.TestClasses
{
	public interface ICircularDependency1
	{
		ICircularDependency2 circularDependency2 { get; }
	}

	public class CircularDependency1 : ICircularDependency1
	{
		public ICircularDependency2 circularDependency2 { get; }

		public CircularDependency1(ICircularDependency2 circularDependency2)
		{
			this.circularDependency2 = circularDependency2;
		}
	}

    public interface ICircularDependency2
    {
        ICircularDependency1 circularDependency1 { get; }
    }

    public class CircularDependency2 : ICircularDependency2
    {
        public ICircularDependency1 circularDependency1 { get; }

        public CircularDependency2(ICircularDependency1 circularDependency1)
        {
            this.circularDependency1 = circularDependency1;
        }
    }
}

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
}

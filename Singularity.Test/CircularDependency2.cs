namespace Singularity.Test.TestClasses
{
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

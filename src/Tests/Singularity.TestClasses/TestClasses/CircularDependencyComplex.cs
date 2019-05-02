namespace Singularity.TestClasses.TestClasses
{
	public interface ICircularDependencyComplex1
	{
	    ICircularDependencyComplex3 circularDependency3 { get; }
	}

	public class CircularDependencyComplex1 : ICircularDependencyComplex1
    {
		public ICircularDependencyComplex3 circularDependency3 { get; }

		public CircularDependencyComplex1(ICircularDependencyComplex3 circularDependency3)
		{
			this.circularDependency3 = circularDependency3;
		}
	}

    public interface ICircularDependencyComplex2
    {
        ICircularDependencyComplex1 circularDependency1 { get; }
    }

    public class CircularDependencyComplex2 : ICircularDependencyComplex2
    {
        public ICircularDependencyComplex1 circularDependency1 { get; }

        public CircularDependencyComplex2(ICircularDependencyComplex1 circularDependency1)
        {
            this.circularDependency1 = circularDependency1;
        }
    }


    public interface ICircularDependencyComplex3
    {
        ICircularDependencyComplex2 circularDependency2 { get; }
    }

    public class CircularDependencyComplex3 : ICircularDependencyComplex3
    {
        public ICircularDependencyComplex2 circularDependency2 { get; }

        public CircularDependencyComplex3(ICircularDependencyComplex2 circularDependency2)
        {
            this.circularDependency2 = circularDependency2;
        }
    }
}

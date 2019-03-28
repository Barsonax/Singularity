using System;

namespace Singularity.TestClasses.TestClasses
{
    public interface ISubObjectOne
    {
    }

    public class SubObjectOne : ISubObjectOne
    {
        public SubObjectOne(IFirstService firstService)
        {
            if (firstService == null)
            {
                throw new ArgumentNullException(nameof(firstService));
            }
        }

        protected SubObjectOne()
        {
        }
    }

    public interface ISubObjectTwo
    {
    }

    public class SubObjectTwo : ISubObjectTwo
    {
        public SubObjectTwo(ISecondService secondService)
        {
            if (secondService == null)
            {
                throw new ArgumentNullException(nameof(secondService));
            }
        }

        protected SubObjectTwo()
        {
        }
    }

    public interface ISubObjectThree
    {
    }

    public class SubObjectThree : ISubObjectThree
    {
        public SubObjectThree(IThirdService thirdService)
        {
            if (thirdService == null)
            {
                throw new ArgumentNullException(nameof(thirdService));
            }
        }

        protected SubObjectThree()
        {
        }
    }
}

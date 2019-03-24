using System;

namespace Singularity.Benchmark.TestClasses
{
    public interface ICombined1
    {
        void DoSomething();
    }

    public interface ICombined2
    {
        void DoSomething();
    }

    public interface ICombined3
    {
        void DoSomething();
    }

    public class Combined1 : ICombined1
    {
        protected static int counter;

        protected Combined1()
        {
        }

        public Combined1(ISingleton1 first, ITransient1 second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Combined");
        }
    }

    public class Combined2 : ICombined2
    {
        protected static int counter;

        protected Combined2()
        {
        }

        public Combined2(ISingleton2 first, ITransient2 second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Combined");
        }
    }

    public class Combined3 : ICombined3
    {
        protected static int counter;

        protected Combined3()
        {
        }

        public Combined3(ISingleton3 first, ITransient3 second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Combined");
        }
    }
}

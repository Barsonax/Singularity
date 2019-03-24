namespace Singularity
{
    public static class Lifetimes
    {
        public static readonly Transient Transient = new Transient();
        public static readonly Singleton Singleton = new Singleton();
    }
}
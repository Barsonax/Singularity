namespace Singularity
{
    public static class ConstructorSelectors
    {
        public static DefaultConstructorSelector Default { get; } = new DefaultConstructorSelector();
        public static MultipleConstructorSelector Multiple { get; } = new MultipleConstructorSelector();
    }
}

namespace Singularity
{
    internal class TypeMetadataCache<T>
    {
        public static readonly bool IsInterface = typeof(T).IsInterface;
    }
}

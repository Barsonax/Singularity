using System;
using System.Collections.Generic;
using System.Text;

namespace Singularity.TestClasses.TestClasses
{
    public class NestedSerializer<T> : INestedSerializer<T>
    {
        public NestedSerializer(T foo)
        {

        }
    }

    public class NestedIntSerializer : INestedSerializer<int>
    {
        public readonly ISerializer<int> IntSerializer;
        public NestedIntSerializer(ISerializer<int> intSerializer)
        {
            IntSerializer = intSerializer;
        }
    }

    public class IntSerializer : ISerializer<int>
    {
    }

    public interface INestedSerializer<T> { }

    public class DefaultSerializer<T> : ISerializer<T>
    {

    }

    public class Special { }

    public class SpecialSerializer : ISerializer<Special>
    {
    }

    public interface ISerializer<T> : ISerializer
    {
    }

    public interface ISerializer
    {
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Singularity.TestClasses.TestClasses
{
    public class ReferenceInt
    {
        public readonly int Value;
        public ReferenceInt(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator ReferenceInt(int value)
        {
            return new ReferenceInt(value);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}

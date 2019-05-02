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

        public override int GetHashCode()
        {
            return Value;
        }
    }
}

namespace Singularity.Collections
{
    internal class ArrayList<T>
    {
        public T[] Array { get; private set; }

        public ArrayList()
        {
            Array = new T[0];
        }

        public ArrayList(T obj)
        {
            Array = new[] {obj};
        }

        public void Add(T obj)
        {
            var newArray = new T[Array.Length + 1];

            for (int i = 0; i < Array.Length; i++)
            {
                newArray[i] = Array[i];
            }

            newArray[newArray.Length - 1] = obj;

            Array = newArray;
        }
    }
}

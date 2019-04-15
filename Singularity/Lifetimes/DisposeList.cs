using System;
using System.Collections.Generic;

namespace Singularity
{
    internal class DisposeList
    {
        private Action<object> Action { get; }
        private List<object> Objects { get; }

        public DisposeList(Action<object> action)
        {
            Action = action;
            Objects = new List<object>(4);
        }

        public void Invoke()
        {
            lock (Objects)
            {
                foreach (object obj in Objects)
                {
                    Action(obj);
                }
            }
        }

        public void Add(object obj)
        {
            lock (Objects)
            {
                Objects.Add(obj);
            }
        }
    }
}
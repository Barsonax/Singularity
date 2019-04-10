using System;
using System.Collections.Generic;

namespace Singularity
{
    internal class DisposeList
    {
        private Action<object> Action { get; }
        private List<object> Objects { get; }
        private readonly object _locker;
        private bool IsDisposed { get; set; }

        public DisposeList(Action<object> action)
        {
            Action = action;
            Objects = new List<object>();
            _locker = new object();
        }

        public void Invoke()
        {
            lock (_locker)
            {
                foreach (object obj in Objects)
                {
                    Action.Invoke(obj);
                }
                Objects.Clear();
                IsDisposed = true;
            }
        }

        public void Add(object obj)
        {
            lock (_locker)
            {
                if (IsDisposed)
                {
                    Action.Invoke(obj);
                }
                else
                {
                    Objects.Add(obj);
                }
            }
        }
    }
}
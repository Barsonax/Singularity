using System;
using System.Collections.Generic;

namespace Singularity
{
    /// <summary>
    /// Represents the scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IDisposable
    {
        private readonly object _locker = new object();
        private Dictionary<Binding, DisposeList> DisposeList { get; } = new Dictionary<Binding, DisposeList>();

        private readonly string _name;
        public Scoped(string name = null)
        {
            _name = name ?? "defaultScope";
        }

        public override string ToString()
        {
            return _name;
        }

        internal void Add(object obj, Binding binding)
        {
            DisposeList list;
            lock (_locker)
            {
                if (!DisposeList.TryGetValue(binding, out list))
                {
                    list = new DisposeList(binding.OnDeathAction);
                    DisposeList.Add(binding, list);
                }
            }
            list.Add(obj);
        }

        /// <summary>
        /// Disposes the scope, calling all dispose actions on the instances that where created in this scope.
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                foreach (DisposeList objectActionList in DisposeList.Values)
                {
                    objectActionList.Invoke();
                }
            }
        }
    }
}

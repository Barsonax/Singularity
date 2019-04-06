using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Singularity
{
    /// <summary>
    /// Represents the scope in which instances will life.
    /// </summary>
    public sealed class Scoped : IDisposable
    {
        private readonly object _locker = new object();
        private ConcurrentDictionary<Type, ObjectActionList> ObjectActionLists { get; } = new ConcurrentDictionary<Type, ObjectActionList>();

        private string _name;
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
            var list = ObjectActionLists.GetOrAdd(binding.Expression.Type, t => new ObjectActionList(binding.OnDeathAction, _locker));
            list.Add(obj);
        }

        /// <summary>
        /// Disposes the scope, calling all dispose actions on the instances that where created in this scope.
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                foreach (ObjectActionList objectActionList in ObjectActionLists.Values)
                {
                    foreach (object obj in objectActionList.Objects)
                    {
                        objectActionList.Action.Invoke(obj);
                    }
                }
            }
        }
    }

    internal readonly struct ObjectActionList
    {
        public Action<object> Action { get; }
        public List<object> Objects { get; }
        private readonly object _locker;

        public ObjectActionList(Action<object> action, object locker)
        {
            Action = action;
            Objects = new List<object>();
            _locker = locker;
        }

        public void Add(object obj)
        {
            lock (_locker)
            {
                Objects.Add(obj);
            }
        }
    }
}

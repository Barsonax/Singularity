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
        private Dictionary<Type, ObjectActionList> ObjectActionLists { get; } = new Dictionary<Type, ObjectActionList>();

        internal ObjectActionList GetActionList(Type type, Action<object> action)
        {
            lock (_locker)
            {
                if (!ObjectActionLists.TryGetValue(type, out ObjectActionList list))
                {
                    list = new ObjectActionList(action, _locker);
                    ObjectActionLists.Add(type, list);
                }

                return list;
            }
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

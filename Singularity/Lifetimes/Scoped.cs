using System;
using System.Collections.Generic;

namespace Singularity
{
    public sealed class Scoped : IDisposable
    {
        private readonly object _locker = new object();
        private Dictionary<Type, ObjectActionList> ObjectActionLists { get; } = new Dictionary<Type, ObjectActionList>();

        public void RegisterAction(Type type, Action<object> action)
        {
            lock (_locker)
            {
                if (!ObjectActionLists.TryGetValue(type, out var list))
                {
                    list = new ObjectActionList(action);
                    ObjectActionLists.Add(type, list);
                }
            }
        }

        public void Add(object obj)
        {
            lock (_locker)
            {
                var type = obj.GetType();
                ObjectActionList objectActionList = ObjectActionLists[type];
                objectActionList.Objects.Add(obj);
            }
        }

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

        private readonly struct ObjectActionList
        {
            public Action<object> Action { get; }
            public List<object> Objects { get; }

            public ObjectActionList(Action<object> action)
            {
                Action = action;
                Objects = new List<object>();
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Singularity.Collections
{
	public class ObjectActionContainer
	{
		private Dictionary<Type, ObjectActionList> ObjectActionLists { get; } = new Dictionary<Type, ObjectActionList>();

		public void AddAction(Type type, Action<object> action)
		{
			ObjectActionLists.Add(type, new ObjectActionList(action));
		}

		public void Add(object obj)
		{
			Type type = obj.GetType();
			ObjectActionList objectActionList = ObjectActionLists[type];
			objectActionList.Objects.Add(obj);
		}

		public void Invoke()
		{
			foreach (ObjectActionList objectActionList in ObjectActionLists.Values)
			{
				foreach (object obj in objectActionList.Objects)
				{
					objectActionList.Action.Invoke(obj);
				}
			}
		}

		private struct ObjectActionList
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
using System;
using System.Collections.Generic;

namespace Singularity.Collections
{
	public class ObjectActionContainer
	{
		public Dictionary<Type, (Action<object> action, List<object> objects)> ActionObjectLists { get; } = new Dictionary<Type, (Action<object> action, List<object> objects)>();

		public void AddAction(Type type, Action<object> action)
		{
			ActionObjectLists.Add(type, (action, new List<object>()));
		}

		public void Add(object obj)
		{
			var type = obj.GetType();
			var list = ActionObjectLists[type];
			list.objects.Add(obj);
		}

		public void Invoke()
		{
			foreach (var (action, objects) in ActionObjectLists.Values)
			{
				foreach (var obj in objects)
				{
					action.Invoke(obj);
				}
			}
		}
	}
}
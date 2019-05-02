using System.Collections.Generic;
using Duality;

namespace Singularity.Duality.Test
{
	public class TestComponentWithDependency : Component
	{
		public IModule Module { get; private set; }
	    public List<IModule> InitCalls { get; } = new List<IModule>();

		public void Init(IModule module)
		{
			Module = module;
            InitCalls.Add(module);
		}
	}
}
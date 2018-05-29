using System;
using System.Collections.Generic;
using System.Text;
using Duality;

namespace Singularity.Duality.Examples
{
    public class GameManagerComponent : Component, IGameManager
    {
	    public int Score { get; }
    }
}

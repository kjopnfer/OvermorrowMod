using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Common.Pathfinding
{
    public static class SharedAIState
    {
        public static PathFinderState State2x2 { get; } = new PathFinderState(2, 2);
        public static PathFinderState State1x2 { get; } = new PathFinderState(1, 2);
    }
}

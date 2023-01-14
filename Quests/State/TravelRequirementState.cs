using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Quests.State
{
    public class TravelRequirementState : CompletableRequirementState
    {
        public TravelRequirementState(TravelRequirement requirement) : base(requirement)
        {
        }
    }
}

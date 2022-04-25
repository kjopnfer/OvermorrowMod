using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public class KillRequirementState : BaseRequirementState
    {
        public KillRequirementState(IQuestRequirement requirement) : base(requirement) { }

        public int NumKilled { get; set; }

        public override void Load(TagCompound tag)
        {
            NumKilled = tag.GetInt("numKilled");
        }

        public override TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["numKilled"] = NumKilled
            };
        }
    }
}

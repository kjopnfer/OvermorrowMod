using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public class KillRequirementState : BaseRequirementState
    {
        public KillRequirementState(IQuestRequirement requirement) : base(requirement) { }

        public Dictionary<int, int> NumKilled { get; private set; } = new Dictionary<int, int>();

        public override void Load(TagCompound tag)
        {
            var keys = tag.GetList<int>("numKilledKeys");
            var values = tag.GetList<int>("numKilledValues");

            NumKilled = keys.Zip(values).ToDictionary(pair => pair.First, pair => pair.Second);
        }

        public override TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["numKilledKeys"] = NumKilled.Keys.ToList(),
                ["numKilledValues"] = NumKilled.Values.ToList()
            };
        }
    }
}

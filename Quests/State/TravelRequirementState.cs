using OvermorrowMod.Core.Interfaces;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    internal class TravelRequirementState : BaseRequirementState
    {
        public bool Traveled { get; set; }

        public TravelRequirementState(IQuestRequirement requirement) : base(requirement) { }


        public override void Load(TagCompound tag)
        {
            Traveled = tag.GetBool("traveled");
        }

        public override TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["traveled"] = Traveled
            };
        }
    }
}

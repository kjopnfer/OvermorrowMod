using OvermorrowMod.Core.Interfaces;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public abstract class BaseRequirementState : TagSerializable
    {
        public IQuestRequirement Requirement { get; }

        public BaseRequirementState(IQuestRequirement requirement)
        {
            Requirement = requirement;
        }

        public abstract void Load(TagCompound tag);

        public abstract TagCompound SerializeData();

        public virtual void Reset()
        {
        }
    }
}
